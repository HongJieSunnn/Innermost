using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Innermost.Identity.API.Models;
using Innermost.Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILoginService<InnermostUser> _loginService;
        private readonly IIdentityServerInteractionService _identityServerInteraction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<InnermostUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            ILoginService<InnermostUser> loginService,
            IIdentityServerInteractionService identityServerInteraction,
            IClientStore clientStore,
            ILogger<AccountController> logger,
            UserManager<InnermostUser> userManager,
            IConfiguration configuration)
        {
            _loginService = loginService;
            _identityServerInteraction = identityServerInteraction;
            _clientStore = clientStore;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }
        /// <summary>
        /// Innermost 注册用户API
        /// </summary>
        /// <param name="model">用户注册信息</param>
        /// <param name="returnUrl">返回的url</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody] RegisterModel model,string returnUrl=null)
        {
            if(ModelState.IsValid)
            {
                var newUser = new InnermostUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Age = model.User.Age,
                    Gender = model.User.Gender,
                    NickName = model.User.NickName,
                    School = model.User.School,
                    Province = model.User.Province,
                    City = model.User.City,
                    SelfDescription = model.User.SelfDescription,
                    Birthday = model.User.Birthday
                };

                var createNewUserRes = await _userManager.CreateAsync(newUser, model.Password);
                if(createNewUserRes.Errors.Count()>0)
                {
                    return BadRequest(await BuildErrorModelStateJsonStr(model, createNewUserRes.Errors.Select(error => error.Description)));
                }
            }

            if(returnUrl!=null&&HttpContext.User.Identity.IsAuthenticated)//returnUrl不为空而且有用户登陆着
            {
                return Redirect(returnUrl);
            }
            //重定向到登录界面
            return Redirect("~/Account/Login");
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _loginService.FindByAccount(loginModel.Account, loginModel.AccountType);

                if(user==null)//用户不存在
                {
                    return Unauthorized("Account is not existing");
                }

                if (await _loginService.ValidateCredentials(user,loginModel.PassWord))
                {
                    var tokenLifetime = _configuration.GetValue("TokenLifetimeMinutes", 120);

                    var authenticationProps = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(tokenLifetime),
                        AllowRefresh = true,
                        RedirectUri = loginModel.ReturnUrl
                    };

                    if(loginModel.RememberMe)
                    {
                        var permanentTokenLifetime = _configuration.GetValue("PermanentTokenLifetimeDays", 365);

                        authenticationProps.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(permanentTokenLifetime);
                        authenticationProps.IsPersistent = true;
                    }

                    await _loginService.SignInAsync(user, authenticationProps);

                    if(_identityServerInteraction.IsValidReturnUrl(loginModel.ReturnUrl))
                    {
                        return Redirect(loginModel.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                return Unauthorized("Account or Password is invalid");//账号密码错误 401
            }

            return BadRequest(await BuildErrorModelStateJsonStr(loginModel,new List<string> { "error model datas." }));
        }
        /// <summary>
        /// 传入的信息json中加入错误信息
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model">post的请求负载对应类型的对象</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public Task<JObject> BuildErrorModelStateJsonStr<TModel>(TModel model, IEnumerable<string> errorMessages)
        {
            var modelJson = JObject.FromObject(model);
            JArray errorArray = new JArray();
            foreach (var error in errorMessages)
            {
                errorArray.Add(error);
            }
            modelJson.Add("errors", errorArray);
            return Task.Run(() =>
            {
                return modelJson;
            });
        }
        /// <summary>
        /// 当登出时，IdentityServer应该会默认调用该函数,并带上logoutId
        /// </summary>
        /// <param name="logoutId">IdentityServer给的</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if(User.Identity.IsAuthenticated==false)
            {
                return await Logout(new LogoutModel { LogoutId = logoutId });
            }

            var logoutContext = await _identityServerInteraction.GetLogoutContextAsync(logoutId);
            if(logoutContext.ShowSignoutPrompt==false)//不显示确认登出界面
            {
                return await Logout(new LogoutModel { LogoutId = logoutId });
            }
            //重定向到确认登出界面，带上logoutId，然后确实登出再带上logoutId来post
            return Redirect($"~/Account/LogoutPrompt?logoutId={logoutId}");
        }
        /// <summary>
        /// 具体登出的操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            //这种是非本地的Identity提供者，也就是例如用谷歌登陆等，需要创建一个logoutId，IdentityServer应该不会自动生成一个，只有使用本地的验证服务器才会
            //这段是参考 eshopContainer的
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    model.LogoutId = await _identityServerInteraction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    await HttpContext.SignOutAsync(idp, new AuthenticationProperties
                    {
                        RedirectUri = url
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LOGOUT ERROR: {ExceptionMessage}", ex.Message);
                }
            }
            //删除cookie
            await HttpContext.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            //登出后设置User为一个anonymous用户
            HttpContext.User = new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity());

            var logoutContext = await _identityServerInteraction.GetLogoutContextAsync(model.LogoutId);
            return Redirect(logoutContext?.PostLogoutRedirectUri);
        }
    }
}

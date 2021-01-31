using IdentityServer4.Services;
using IdentityServer4.Stores;
using Innermost.Identity.API.Models;
using Innermost.Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _loginService.FindByAccount(loginModel.Account, loginModel.AccountType);

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

            return BadRequest(await BuildErrorModelStateJsonStr(loginModel,"error model datas."));
        }

        public Task<JObject> BuildErrorModelStateJsonStr<TModel>(TModel model, string errorMessage)
        {
            var modelJson = JObject.FromObject(model);
            modelJson.Add("error", errorMessage);
            return Task.Run(() =>
            {
                return modelJson;
            });
        }
    }
}

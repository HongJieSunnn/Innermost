using Innermost.Identity.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Services
{
    /// <summary>
    /// Innermost Login Service
    /// </summary>
    public class InnermostLoginService : ILoginService<InnermostUser>
    {
        private readonly UserManager<InnermostUser> _userManager;
        private readonly SignInManager<InnermostUser> _signInManager;
        public InnermostLoginService(UserManager<InnermostUser> userManager,SignInManager<InnermostUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<InnermostUser> FindByAccount(string account, string type)
        {
            if (type == "Email")
                return await _userManager.FindByNameAsync(account);
            else if (type == "UserName")
                return await _userManager.FindByEmailAsync(account);
            throw new ArgumentException("Type param must be Email or UserName");
        }

        public Task SignInAsync(InnermostUser user)
        {
            return _signInManager.SignInAsync(user, true);
        }

        public Task SignInAsync(InnermostUser user, AuthenticationProperties properties, string authenticationMethod = null)
        {
            return _signInManager.SignInAsync(user, properties, authenticationMethod);
        }

        public async Task<bool> ValidateCredentials(InnermostUser user, string pwd)
        {
            return await _userManager.CheckPasswordAsync(user, pwd);
        }
    }
}

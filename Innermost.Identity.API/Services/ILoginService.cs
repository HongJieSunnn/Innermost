using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Services
{
    public interface ILoginService<TUser>
    {
        /// <summary>
        /// Validate user and password
        /// </summary>
        /// <param name="user">template TUser's instance</param>
        /// <param name="pwd">the password of user</param>
        /// <returns></returns>
        Task<bool> ValidateCredentials(TUser user, string pwd);
        /// <summary>
        /// Account may be email or username and there are different method for those two kind of account.
        /// So this method can call the correct method by type param.
        /// </summary>
        /// <param name="account">user account</param>
        /// <param name="type">user type.Email/UserName</param>
        /// <returns></returns>
        Task<TUser> FindByAccount(string account, string type);
        /// <summary>
        /// SignIn by user model instance.Persist cookie in browers.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task SignInAsync(TUser user);
        /// <summary>
        /// SignIn by user model instance and custom authenticationproperties.Cookie persisted policy depends on the properties param.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <param name="authenticationMethod"></param>
        /// <returns></returns>
        Task SignInAsync(TUser user, AuthenticationProperties properties, string authenticationMethod = null);
    }
}

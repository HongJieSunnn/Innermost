using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Services.IdentityServices
{
    public class IdentityService
        : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public string GetUserId()
        {
            //若要使用 FindFirst("sub") 需要先在 Startup 中 JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub"); 否则，需要 User.FindFirstValue(ClaimTypes.NameIdentifier);来获取
            return _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst("preferred_username").Value;
        }
    }
}

using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Configuration
{
    public class Config
    {
        /// <summary>
        /// Apis in Innermost
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("loglife","LogLife Api"),
            };
        }
        /// <summary>
        /// Get IdentityResource like userID、Profile、email which needs to be protected.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
        /// <summary>
        /// Clients want to access resources
        /// </summary>
        /// <param name="clientUrls">clients' url dictionary key:clientId value:url.The data is in appsettings.json</param>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients(Dictionary<string,string> clientUrls)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="webapp",
                    ClientName="Web FrontEnd Of Innermost",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser=true,
                    RequireConsent=false,//needn't consent screen (consent what profile you agree to be accessed)
                    RedirectUris={ $"{clientUrls["WebApp"]}/"},
                    PostLogoutRedirectUris={$"{clientUrls["WebApp"]}/"},
                    AllowedCorsOrigins={$"{clientUrls["WebApp"]}"},
                    AllowedScopes=
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "loglife"
                    },
                    AccessTokenLifetime=60*60*3,
                    IdentityTokenLifetime=60*60*3
                },
                new Client
                {
                    ClientId="mobileapp",
                    ClientName="Mobile App Of Innermost",
                    ClientSecrets={new Secret("mobileappInnermost".Sha256())},
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser=false,
                    AllowOfflineAccess=true,
                    AccessTokenLifetime=60*60*2,
                    IdentityTokenLifetime=60*60*2,
                    RequireConsent=false,
                    RequirePkce=true,
                    RedirectUris = { clientUrls["MobileApp"] },
                    PostLogoutRedirectUris = { clientUrls["MobileApp"] },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "loglife"
                    },
                }
            };
        }
    }
}

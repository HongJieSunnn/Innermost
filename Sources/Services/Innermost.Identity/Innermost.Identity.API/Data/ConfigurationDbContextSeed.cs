using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Innermost.Identity.API.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Data
{
    /// <summary>
    /// Seed data of ConfigurationDb.
    /// This db register in StartUp by AddConfigurationStore Method.
    /// </summary>
    public class ConfigurationDbContextSeed
    {
        /// <summary>
        /// Seed database async.
        /// </summary>
        /// <param name="db">ConfigurationDbContext instance</param>
        /// <param name="configuration">configurations in appsettings.json file</param>
        /// <returns></returns>
        public async Task SeedAsync(ConfigurationDbContext db, IConfiguration configuration)
        {
            var redirectUrls = new Dictionary<string, string>();

            redirectUrls.Add("WebApp", configuration["RedirectUrls:WebappClient"]);
            redirectUrls.Add("MobileApp", configuration["RedirectUrls:MobileappClient"]);

            //this method just add the seed data so that we should not add the function to add new data into configurationdb in this method.
            if (!db.Clients.Any())
            {
                foreach (var client in Config.GetClients(redirectUrls))
                {
                    db.Clients.Add(client.ToEntity());
                }
                await db.SaveChangesAsync();
            }
            else
            {
                //there may cause some problem about swagger-ui
                //see ref: https://github.com/dotnet-architecture/eShopOnContainers/issues/586
            }

            if(!db.IdentityResources.Any())
            {
                foreach (var resource in Config.GetResources())
                {
                    db.IdentityResources.Add(resource.ToEntity());
                }
                await db.SaveChangesAsync();
            }

            if(!db.ApiResources.Any())
            {
                foreach (var api in Config.GetApis())
                {
                    db.ApiResources.Add(api.ToEntity());
                }
                await db.SaveChangesAsync();
            }

            if (!db.ApiScopes.Any())
            {
                foreach (var scope in Config.GetApiScopes())
                {
                    db.ApiScopes.Add(scope.ToEntity());
                }
                await db.SaveChangesAsync();
            }
        }
    }
}

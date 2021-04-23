using IdentityServer4.EntityFramework.DbContexts;
using Innermost.Identity.API.Services;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Factories
{
    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            var options = FactoryService.GetDbContextOptionsMySQL<ConfigurationDbContext>();

            return new ConfigurationDbContext(options,new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions());
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Services
{
    public class FactoryService
    {
        public static DbContextOptions<TContext> GetDbContextOptionsMySQL<TContext>() where TContext:DbContext
        {
            var config = ConfigurationService.GetConfiguration();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            optionsBuilder.UseMySql(config.GetConnectionString("ConnectMySQL"), new MySqlServerVersion(new Version(5, 7)), options =>
            {
                options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            });

            return optionsBuilder.Options;
        }
    }
}

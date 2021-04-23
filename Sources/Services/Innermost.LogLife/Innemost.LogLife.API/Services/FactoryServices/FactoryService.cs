﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Services.FactoryServices
{
    public class FactoryService
    {
        public static DbContextOptions<TContext> GetDbContextOptions<TContext>() where TContext:DbContext
        {
            var configuration = Program.GetConfiguration();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            optionsBuilder.UseMySql(configuration.GetConnectionString("ConnectMySQL"), new MySqlServerVersion(new Version(5, 7)), options =>
            {
                options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            });

            return optionsBuilder.Options;
        }
    }
}

using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusInnermost.Abstractions;
using Innemost.LogLife.API.Infrastructure.Filters;
using Innermost.EventBusInnermost;
using Innermost.EventBusInnermost.Abstractions;
using Innermost.EventBusServiceBus;
using Innermost.LogLife.Infrastructure;
using IntegrationEventRecord.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Innemost.LogLife.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .Services
                .AddHealthCheck(Configuration)
                .AddCustomDbContext(Configuration)
                .AddCustomEventBus(Configuration)
                .AddCustomIntegrationEventConfiguration(Configuration)
                .AddCustomAuthentication(Configuration);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Innemost.LogLife.API", Version = "v1" });
            });

            var container = new ContainerBuilder();

            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Innemost.LogLife.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            ConfigureAuth(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthCheck(this IServiceCollection services,IConfiguration configuration)
        {
            var healthCheckBuilder = services.AddHealthChecks();

            healthCheckBuilder.AddCheck("loglife", () => HealthCheckResult.Healthy());//对该服务进行检查，第二个参数实际上是检查的具体逻辑，由于能检查到那么一定健康，所以返回Healthy就好。

            healthCheckBuilder
                .AddMySql(
                    configuration["ConnectMySQL"], 
                    "Innermost.LogLifeMySQLDB-Check", 
                    tags: new string[] { "loglifemysqldb" });

            healthCheckBuilder
                .AddAzureServiceBusTopic(
                    configuration["ConnectAzureServiceBus"],
                    "innermost_event_bus",
                    "Innermost.LogLife-AzureServiceBus-Check",
                    tags: new string[] { "loglifeservicebus" });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,IConfiguration configuration)
        {
            services
                .AddDbContext<LifeRecordDbContext>(options =>
                {
                    options
                        .UseMySql(configuration["ConnectMySQL"], new MySqlServerVersion(new Version(5, 7)), options =>
                         {
                             options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                             options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         });
                });

            services
                .AddDbContext<LifeRecordDbContext>(options =>
                {
                    options
                        .UseMySql(configuration["ConnectMySQL"], new MySqlServerVersion(new Version(5, 7)), options =>
                        {
                            options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                            options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                });

            //TODO redis/mongodb/log

            return services;
        }

        public static IServiceCollection AddCustomIntegrationEventConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            //TODO
            services.AddTransient<IntegrationEventRecordServiceFactory>(); 

            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var connectionString = configuration["ConnectAzureServiceBus"];
                var logger = sp.GetService<ILogger<DefaultServiceBusPersisterConnection>>();

                return new DefaultServiceBusPersisterConnection(connectionString, logger);
            });

            return services;
        }

        public static IServiceCollection AddCustomEventBus(this IServiceCollection services,IConfiguration configuration)
        {
            var subcriptionName = configuration["SubscriptionClientName"];

            services.AddSingleton<IAsyncEventBus, EventBusServiceBus>(sp =>
            {
                var persister = sp.GetRequiredService<IServiceBusPersisterConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                var subcriptionManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var lifescope = sp.GetRequiredService<ILifetimeScope>();

                return new EventBusServiceBus(persister, logger, subcriptionManager,subcriptionName, lifescope);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            var identityServerUrl = configuration["IdentityServerUrl"];

            services
                .AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = identityServerUrl;
                    options.Audience = "LogLife";
                });

            return services;
        }
    }
}

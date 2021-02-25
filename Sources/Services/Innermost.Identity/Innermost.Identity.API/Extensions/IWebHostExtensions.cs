using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Extensions
{
    /// <summary>
    /// Extensions class of IWebHost Interface
    /// </summary>
    public static class IWebHostExtensions
    {
        /// <summary>
        /// judge whether in K8S Env
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static bool IsK8S(this IWebHost webHost)
        {
            var config = (IConfiguration) webHost.Services.GetService(typeof(IConfiguration));
            var orchestratorType = config.GetValue<string>("OrchestratorType");
            return orchestratorType?.ToUpper() == "K8S";//? stands for this object may be null
        }
        /// <summary>
        /// To Migrate DbContext
        /// </summary>
        /// <typeparam name="TDbContext">The type of DbContext to migrate</typeparam>
        /// <param name="webHost"></param>
        /// <param name="seeder">The action to initial db by seed data</param>
        /// <returns></returns>
        public static IWebHost MigrateDbContext<TDbContext>(this IWebHost webHost,Action<TDbContext,IServiceProvider> seeder) where TDbContext:DbContext
        {
            var underK8S = IsK8S(webHost);

            using (var scope=webHost.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var logger = service.GetRequiredService<ILogger<TDbContext>>();
                var context = service.GetRequiredService<TDbContext>();

                try
                {
                    if(underK8S)//if underk8s we need not to set retry policy 因为如果处于k8s我们不需要通过Policy来重试，直接抛出异常，k8s会重启服务的。也就是实际上未必处于k8s环境，但不处于时我们要额外设置重试的方法。
                    {
                        InvokeSeeder(seeder, context, service);
                    }
                    else
                    {
                        var retries = 5;
                        var retry = Policy.Handle<MySqlException>()
                            .WaitAndRetry(
                                retryCount:retries,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry:(exception,timeSpan,retry,context)=>
                                {
                                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(TDbContext), exception.GetType().Name, exception.Message, retry, retries);
                                });
                        retry.Execute(() => InvokeSeeder(seeder, context, service));//这个一样是执行了初始化种子数据，只是如果失败了也通过该方法来重试，而K8S环境下则不需要。
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TDbContext).Name);
                    if (underK8S)
                    {
                        throw;          // Rethrow under k8s because we rely on k8s to re-run the pod
                    }
                }
            }
            return webHost;
        }
        /// <summary>
        /// Migrate dbcontext and initial database by seeder action。
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="seeder"></param>
        /// <param name="dbContext"></param>
        /// <param name="service"></param>
        private static void InvokeSeeder<TDbContext>(Action<TDbContext, IServiceProvider> seeder,TDbContext dbContext,IServiceProvider service) where TDbContext:DbContext
        {
            dbContext.Database.Migrate();
            seeder(dbContext, service);
        }
    }
}

using IdentityServer4.EntityFramework.DbContexts;
using Innermost.Identity.API;
using Innermost.Identity.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 使用顶级语句
/// </summary>

string Namespace = typeof(Startup).Namespace;
string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", AppName);
    var host = CreateHostBuilder(args, configuration);

    Log.Information("Applying migrations ({ApplicationContext})...", AppName);
    host.MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
        .MigrateDbContext<InnermostIdentityDbContext>((_, __) => { })
        .MigrateDbContext<ConfigurationDbContext>((dbContext, services) =>
        {
            //MigrateDbContext 是 IWebHost 的拓展函数，那么该函数可以使用调用它的 IWebHost 实例中的数据，这里就可以使用实例 host 中的数据
            //service 只是个形参，在MigrateDbContext会通过 IWebHost 实例获取到 IServiceProvider 的实例来初始化 service，进而通过 service 获取对应的 DbContext 实例
            //然后 MigrateDbContext 会 Invoke 我们传入的 seeder 函数表达式。这样初始化数据库的工作就由 MigrateDbContext 来做，而不需要这里来做。
            new ConfigurationDbContextSeed()
                .SeedAsync(dbContext, configuration)
                .Wait();
        });

    Log.Information("Starting web host ({ApplicationContext})...", AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}



IWebHost CreateHostBuilder(string[] args, IConfiguration configuration)
{
    return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration(c => c.AddConfiguration(configuration))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .Build();
}


Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://localhost:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    var config = builder.Build();

    return builder.Build();
}

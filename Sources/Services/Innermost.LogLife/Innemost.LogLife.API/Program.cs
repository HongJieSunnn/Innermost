using Innemost.LogLife.API;
using Innemost.LogLife.API.Infrastructure.SeedDatas;
using Innermost.LogLife.Infrastructure;
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
using System.Reflection;
using System.Threading.Tasks;

IConfiguration configuration = Program.GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);
    var host = CreateWebHostBuilder(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", Program.AppName);
    host.MigrateDbContext<LifeRecordDbContext>((context,service)=>
    {
        new LifeRecordDbContextSeed()
            .SeedAsync(context,configuration)
            .Wait();
    });

    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

IWebHost CreateWebHostBuilder(IConfiguration configuration,string[] args)
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
        .Enrich.WithProperty("ApplicationContext", Program.AppName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://localhost:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

public class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

    public static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables();//no environmentvariables in this service

        var config = builder.Build();

        return config;
    }
}
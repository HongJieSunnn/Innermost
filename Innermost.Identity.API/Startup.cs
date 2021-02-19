using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Innermost.Identity.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Innermost.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Innermost.Identity.API.Services;

namespace Innermost.Identity.API
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
            var sqlConnectionString = Configuration.GetConnectionString("ConnectMySQL");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //EF Core MySQL ��������
            services.AddDbContext<InnermostIdentityDbContext>(options => 
                options.UseMySql(
                    sqlConnectionString,
                    new MySqlServerVersion(new Version(5,7)),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    }
                )
            );

            //���ݿ⽡�����
            services.AddHealthChecks()
                .AddCheck("self", ()=>HealthCheckResult.Healthy())
                .AddMySql(sqlConnectionString,
                    name: "IdentityDB-Check",
                    tags: new string[] { "IdentityDB" });
            //���transient����
            services.AddTransient<ILoginService<InnermostUser>, InnermostLoginService>();

            //��� ASP.NET Identity
            services.AddIdentity<InnermostUser, IdentityRole>()
                .AddEntityFrameworkStores<InnermostIdentityDbContext>()
                .AddDefaultTokenProviders();

            //��� IdentityServer
            var builder = services.AddIdentityServer(options =>
              {
                  options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
              })
            .AddAspNetIdentity<InnermostUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseMySql(sqlConnectionString,
                    new MySqlServerVersion(new Version(5, 7)),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseMySql(sqlConnectionString,
                    new MySqlServerVersion(new Version(5, 7)),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(migrationsAssembly);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            //���˺��������Ϣ����
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
                options.User.RequireUniqueEmail = true;
            });

            //����cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "InnermostUserCookie";
                options.LoginPath = "/Account/Login";
            });

            //����ʹ�õ�֤�飬����������������Ҫ��eshop��Ŀ��һ��Ūһ��֤��װ��ȥ
            builder.AddDeveloperSigningCredential();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Innermost.Identity.API", Version = "v1" });
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Innermost.Identity.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

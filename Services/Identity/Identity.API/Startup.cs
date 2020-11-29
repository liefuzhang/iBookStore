using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Services;
using Identity.API.Configuration;
using System.Reflection;
using iBookStoreCommon.Extensions;
using iBookStoreCommon.Helper;
using iBookStoreCommon.Infrastructure;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using Identity.API.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger<Startup> _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = CommonHelper.GetConnectString(Configuration["ConnectionString"]);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            null);
                    }));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(config =>
                {
                    config.Filters.AddService<RequestResponseLoggingFilter>();
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();

            services.AddScoped<RequestResponseLoggingFilter>();

            //// Adds IdentityServer
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              // this adds the operational data from DB (codes, tokens, consents)
              .AddOperationalStore(options =>
              {
                  options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                        });
                  // this enables automatic token cleanup. this is optional.
                  options.EnableTokenCleanup = true;
                  options.TokenCleanupInterval = 3600; // interval in seconds
              })
              .AddInMemoryIdentityResources(Config.GetIdentityResources())
              .AddInMemoryApiResources(Config.GetApiResources())
              .AddInMemoryClients(Config.GetClients(Configuration))
              .AddAspNetIdentity<ApplicationUser>()
              .Services.AddTransient<IProfileService, ProfileService>(); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseForwardedHeadersCommon();

            app.UseMiddleware<GlobalTraceLoggingMiddleware>();

            app.UseIdentityServer();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

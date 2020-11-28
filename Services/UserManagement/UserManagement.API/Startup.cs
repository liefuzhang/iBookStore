using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus;
using iBookStoreCommon;
using iBookStoreCommon.Extensions;
using iBookStoreCommon.Infrastructure;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using iBookStoreCommon.ServiceRegistry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagement.API.Infrastructure;
using UserManagement.API.Service;

namespace Recommendation.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCommonIBookStoreServices(Configuration);

            services.AddDbContext<UserManagementContext>(options =>
            {
                options.UseNpgsql(Configuration["ConnectionString"],
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                    });
            });
            
            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp => {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName, Configuration["MessageQueueUrl"]);
            });

            ConfigureAuthService(services);

            services.AddTransient<INewsletterService, NewsletterService>();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "userManagement";
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCommonIBookStoreServices(env, Configuration, false, true);
        }
    }
}

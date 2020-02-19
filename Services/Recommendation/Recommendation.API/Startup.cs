using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using iBookStoreCommon;
using iBookStoreCommon.Infrastructure;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using iBookStoreCommon.ServiceRegistry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recommendation.API.Infrastructure;
using Recommendation.API.IntegrationEvents.EventHandling;
using Recommendation.API.IntegrationEvents.Events;
using Recommendation.API.Services;

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
            services.Configure<AppSettings>(Configuration);

            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddMvc(config =>
                {
                    config.Filters.AddService<RequestResponseLoggingFilter>();
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp => {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName);
            });
            services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();

            services.AddScoped<RequestResponseLoggingFilter>();

            ConfigureAuthService(services);

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<ServiceRegistryRepository>();
            services.AddTransient<ServiceRegistryRegistrationService>();

            services.AddHttpClient<ICatalogService, CatalogService>();
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
                options.Audience = "recommendation";
            });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<GlobalTraceLoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseMvc();

            ConfigureEventBus(app);

            RegisterService(app, Configuration);
        }
        
        private static void RegisterService(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceRegistryRegistrationService =
                    scope.ServiceProvider.GetRequiredService<ServiceRegistryRegistrationService>();
                serviceRegistryRegistrationService.Initialize(configuration["ApplicationName"], new Uri(configuration["ApplicationUri"]));
            }
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
        }
    }
}

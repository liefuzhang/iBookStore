using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.Infrastructure;
using Basket.API.IntegrationEvents.EventHandling;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Services;
using EventBus;
using iBookStoreCommon;
using iBookStoreCommon.Extensions;
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
using Payment.API.IntegrationEvents.Events;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<AppSettings>(Configuration);

            services.ConfigureCommonIBookStoreServices(Configuration);
            
            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp => {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName, Configuration["MessageQueueUrl"]);
            });

            services.AddScoped<OrderStartedIntegrationEventHandler>();
            services.AddScoped<ProductPriceChangedIntegrationEventHandler>();
            
            services.AddHttpClient<ICatalogService, CatalogService>();
            services.AddHttpClient<IOrderService, OrderService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            //services.AddDistributedMemoryCache();

            ConfigureAuthService(services);

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            services.AddTransient<IWishlistRepository, WishlistRepository>();
        }

        private void ConfigureAuthService(IServiceCollection services) {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "basket";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseCommonIBookStoreServices(env, Configuration, false, true, useForwordedHeaders:true);

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app) {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
            eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        }
    }
}

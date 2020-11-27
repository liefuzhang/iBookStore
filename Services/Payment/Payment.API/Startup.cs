using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using iBookStoreCommon;
using iBookStoreCommon.Extensions;
using iBookStoreCommon.Infrastructure;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.API.IntegrationEvents.EventHandling;
using Payment.API.IntegrationEvents.Events;

namespace Payment.API
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

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp => {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName, Configuration["MessageQueueUrl"]);
            });
            services.AddScoped<OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseCommonIBookStoreServices(env, Configuration, false, false);

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app) {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
        }
    }
}

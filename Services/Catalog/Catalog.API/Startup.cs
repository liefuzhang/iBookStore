using System;
using System.Data.Common;
using System.Reflection;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents;
using Catalog.API.IntegrationEvents.EventHandling;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.Services;
using EventBus;
using iBookStoreCommon.Extensions;
using iBookStoreCommon.Helper;
using IntegrationEventLogEF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Catalog.API
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

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var queueName = Configuration["MessageQueueName"];

                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName, Configuration["MessageQueueUrl"]);
            });

            var connectionString = CommonHelper.GetConnectString(Configuration["ConnectionString"]);

            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseNpgsql(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                });
            });

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), null);
                    });
            });

            services.AddCustomHealthCheck(Configuration);

            services.AddScoped<OrderStatusChangedToPaidIntegrationEventHandler>();

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            services.AddHttpClient<ICatalogItemRatingService, CatalogItemRatingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCommonIBookStoreServices(env, Configuration, true, false);

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = CommonHelper.GetConnectString(configuration["ConnectionString"]);
            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddSqlServer(
                    connectionString,
                    name: "CatalogDB-check",
                    tags: new string[] { "catalogdb" });

            hcBuilder
                .AddRabbitMQ(
                    configuration["MessageQueueUrl"],
                    name: "catalog-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });

            return services;
        }
    }
}

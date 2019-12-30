using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using EventBus;
using iBookStoreCommon;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API.Application.IntegrationEvents.EventHandling;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.API.Application.MediatRBehaviors;
using Ordering.API.Application.Queries;
using Ordering.API.Services;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.API
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

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddDbContext<OrderingContext>(options =>
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureAuthService(services);

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName);
            });
            services.AddHttpClient<ICatalogService, CatalogService>();
            services.AddScoped<GracePeriodConfirmedIntegrationEventHandler>();
            services.AddScoped<OrderPaymentFailedIntegrationEventHandler>();
            services.AddScoped<OrderPaymentSucceededIntegrationEventHandler>();

            services.AddSingleton<IOrderQueries>(s => new OrderQueries(Configuration["ConnectionString"]));

            services.AddScoped<IRequestManager, RequestManager>();

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddTransient<ServiceRegistryRepository>();
            services.AddTransient<ServiceRegistryRegistrationService>();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "ordering";
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
                serviceRegistryRegistrationService.Initialize(app, configuration["ApplicationName"]);
            }
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, GracePeriodConfirmedIntegrationEventHandler>();
            eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
            eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
        }
    }
}

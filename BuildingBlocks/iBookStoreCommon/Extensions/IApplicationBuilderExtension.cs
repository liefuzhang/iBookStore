using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using EventBus;
using HealthChecks.UI.Client;
using iBookStoreCommon.Infrastructure;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using iBookStoreCommon.ServiceRegistry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace iBookStoreCommon.Extensions
{
    public static class IApplicationBuilderExtension
    {
        public static void UseCommonIBookStoreServices(this IApplicationBuilder app,
            IHostingEnvironment env, IConfiguration configuration, bool useHealthCheck = false,
            bool useAuthtication = false, bool useForwordedHeaders = false)
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

            if (useHealthCheck)
            {

                app.UseHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                app.UseHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{configuration["ApplicationName"]} V1");
            });

            app.UseCors("CorsPolicy");

            if (useAuthtication)
            {
                app.UseAuthentication();
            }

            app.UseMvc();

            RegisterService(app, configuration);
        }

        public static void UseForwardedHeadersCommon(this IApplicationBuilder app)
        {
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            // ref: https://github.com/aspnet/Docs/issues/2384
            app.UseForwardedHeaders(forwardOptions);
        }

        private static void RegisterService(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceRegistryRegistrationService =
                    scope.ServiceProvider.GetRequiredService<ServiceRegistryRegistrationService>();
                serviceRegistryRegistrationService.Initialize(
                        configuration["ApplicationName"],
                        new Uri(configuration["ApplicationUrl"]),
                        configuration["ApiGatewayUrl"])
                    .GetAwaiter()
                    .GetResult();
            }
        }

    }
}

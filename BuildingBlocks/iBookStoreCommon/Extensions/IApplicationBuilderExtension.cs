using HealthChecks.UI.Client;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace iBookStoreCommon.Extensions
{
    public static class IApplicationBuilderExtension
    {
        public static void UseCommonIBookStoreServices(this IApplicationBuilder app,
            IHostingEnvironment env, IConfiguration configuration, bool useHealthCheck,
            bool useAuthtication)
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
        }
    }
}

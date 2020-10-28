using iBookStoreCommon.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace iBookStoreCommon.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static void ConfigureCommonIBookStoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddMvc(config =>
            {
                config.Filters.AddService<RequestResponseLoggingFilter>();
                config.Filters.AddService<HttpResponseExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = configuration["ApplicationName"], Version = "v1" });
            });

            services.AddScoped<RequestResponseLoggingFilter>();
            services.AddScoped<HttpResponseExceptionFilter>();

            services.AddHttpContextAccessor();
        }
    }
}

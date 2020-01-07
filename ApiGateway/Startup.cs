using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway;
using ApiGateway.Infrastructure;
using ApiGateway.Repositories;
using ApiGateway.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iBookStoreApiGateway
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
            services.Configure<ApiGatewaySettings>(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IServiceRegistryService, ServiceRegistryService>();
            services.AddScoped<IServiceRegistryRepository, ServiceRegistryRepository>();
            services.AddTransient<IServiceOperationService, ServiceOperationService>();

            services.AddHttpClient(nameof(ReverseProxyMiddleware));
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
            app.UseMvc();

            app.UseMiddleware<ReverseProxyMiddleware>();
            // API Gateway ReverseProxy is the ultimate end of our middleware pipeline
            // Nothing should be below this.
            // ---------------------------------------------------------------------------------------------------
        }
    }
}

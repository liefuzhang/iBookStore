using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using iBookStoreCommon.Infrastructure.Vocus.Common.AspNetCore.Logging.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.BackgroundTasks.Services;
using Ordering.BackgroundTasks.Tasks;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Ordering.BackgroundTasks
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<BackgroundTaskSettings>(Configuration);
            services.AddOptions();

            services.AddHttpClient<IOrderService, OrderService>();

            services.AddSingleton<IHostedService, GracePeriodManagerService>();

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp => {
                var queueName = Configuration["MessageQueueName"];
                return new EventBusRabbitMQ.EventBusRabbitMQ(sp, queueName, Configuration["MessageQueueUrl"]);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GlobalTraceLoggingMiddleware>();
        }
    }
}

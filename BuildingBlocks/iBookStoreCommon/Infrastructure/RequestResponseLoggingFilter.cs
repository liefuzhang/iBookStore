using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace iBookStoreCommon.Infrastructure
{
    /// <summary>
    /// Logs all Requests and Responses to MVC Controllers.
    /// </summary>
    public class RequestResponseLoggingFilter : ActionFilterAttribute, IDisposable
    {
        private readonly ILogger _logger;

        public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger)
        {
            _logger = logger;
        }

        /// <remarks>Before</remarks>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(0, $"Enter - {context.ActionDescriptor.DisplayName}.");
        }

        /// <remarks>After</remarks>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            switch (context.Exception)
            {
                case null:
                    _logger.LogInformation($"Complete - {context.ActionDescriptor.DisplayName}.");
                    break;

                default:
                    _logger.LogError(context.Exception, $"{context.Exception.GetType().Name} - {context.Exception.Message}");
                    break;
            }
        }
        
        public void Dispose()
        {
        }
    }
}

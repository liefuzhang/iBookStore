using System;
using System.Collections.Generic;
using System.Text;

namespace iBookStoreCommon.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Logging;

    namespace Vocus.Common.AspNetCore.Logging.Middleware
    {
        /// <summary>
        /// Provides global logging in the ASPNETCore Middleware pipeline.
        /// This is to supplement the logging in the MVC pipeline.
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Global instantiated by ASPNETCore
        public class GlobalTraceLoggingMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger _logger;

            public GlobalTraceLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
            {
                _next = next;
                _logger = loggerFactory?.CreateLogger<GlobalTraceLoggingMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            }

            public async Task Invoke(HttpContext context)
            {
                HttpRequestInfo request = null;
                try
                {
                    request = new HttpRequestInfo(context.Request);

                    _logger.LogTrace("Enter - {Request:l}", request);

                    await _next(context);

                    _logger.LogTrace("Complete - {Request:l}", request);

                }
                catch (Exception ex)
                {
                    // this means an exception was thrown up the middleware pipeline and not handled and transformed into a 
                    // 4/5xx response by MVC. 
                    _logger.LogCritical(ex, "Exception - {Request:l}", request);

                    // rethrow for the global exception handler to transform in to a valid response
                    throw;
                }
            }
        }

        /// <summary>
        /// Stripped-down version of the <see cref="HttpRequest"/> for logging only.
        /// </summary>
        internal class HttpRequestInfo
        {
            public HttpRequestInfo(HttpRequest request)
            {
                if (request == null) { throw new ArgumentNullException(nameof(request)); }

                Scheme = request.Scheme;
                Host = request.Host;
                Method = request.Method;
                PathBase = request.PathBase;
                Path = request.Path;
                Query = request.QueryString;
            }

            public string Scheme { get; }

            public HostString Host { get; }

            public string Method { get; }

            public PathString PathBase { get; }

            public PathString Path { get; }

            public QueryString Query { get; }

            public override string ToString()
            {
                return Method + " " + UriHelper.BuildAbsolute(Scheme, Host, PathBase, Path, Query);
            }
        }
    }
}

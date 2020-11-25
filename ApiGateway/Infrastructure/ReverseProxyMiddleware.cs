using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace ApiGateway.Infrastructure
{
    public class ReverseProxyMiddleware
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReverseProxyMiddleware> _logger;
        private readonly ApiGatewaySettings _settings;

        private const string Bearer = "Bearer";
        private const string XForwardedForHeaderKey = "X-Forwarded-For";

        public ReverseProxyMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory,
            IOptions<ApiGatewaySettings> apiGatewaySettingsOptions,
            IHttpClientFactory httpClientFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<ReverseProxyMiddleware>();
            _settings = apiGatewaySettingsOptions?.Value ??
                        throw new ArgumentNullException(nameof(apiGatewaySettingsOptions));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Request == null) throw new Exception("The HttpContext.Request was NULL, we cannot proxy an empty request.");

            var path = context.Request.Path.ToString();
            var pattern = @"\/api\/([a-zA-Z]+)\S*";

            var match = Regex.Match(path, pattern, RegexOptions.IgnoreCase);

            if (!match.Success) 
                throw new Exception("The requested path is invalid.");

            var serviceHost = $"{match.Groups[1].Value}.api";

            await ProxyRequestAsync(context, serviceHost);
        }

        protected virtual async Task ProxyRequestAsync(HttpContext context, string serviceHost)
        {
            using (var requestMessage = new HttpRequestMessage())
            {
                var requestMethod = context.Request.Method;
                if (!HttpMethods.IsGet(requestMethod)
                    && !HttpMethods.IsHead(requestMethod)
                    && !HttpMethods.IsDelete(requestMethod)
                    && !HttpMethods.IsTrace(requestMethod))
                {
                    var streamContent = new StreamContent(context.Request.Body);
                    requestMessage.Content = streamContent;
                }

                var xForwardedForHeaderValues = new List<string>();

                // copy the request headers
                foreach (var header in context.Request.Headers)
                {
                    if (string.Equals(header.Key, XForwardedForHeaderKey, StringComparison.OrdinalIgnoreCase))
                    {
                        xForwardedForHeaderValues = header.Value.ToList();
                        continue;
                    }

                    if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    {
                        requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }

                // add remote ip address to x-forwarded-for header
                xForwardedForHeaderValues.Add(context.Connection.RemoteIpAddress.ToString());
                requestMessage.Headers.TryAddWithoutValidation(XForwardedForHeaderKey, xForwardedForHeaderValues);

                var requestUri = new Uri($"{context.Request.Scheme}://{serviceHost}{context.Request.Path}{context.Request.QueryString}");
                requestMessage.Headers.Host = requestUri.Authority;
                requestMessage.RequestUri = requestUri;
                requestMessage.Method = new HttpMethod(requestMethod);

                _logger.LogInformation($"ProxyRequestAsync RequestUri: {requestUri}");

                var httpClient = _httpClientFactory.CreateClient(nameof(ReverseProxyMiddleware));
                using (var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
                {
                    context.Response.StatusCode = (int)responseMessage.StatusCode;

                    foreach (var header in responseMessage.Headers)
                    {
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                    }

                    foreach (var header in responseMessage.Content.Headers)
                    {
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                    }

                    // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
                    context.Response.Headers.Remove("transfer-encoding");

                    await responseMessage.Content.CopyToAsync(context.Response.Body);
                }
            }
        }

    }
}

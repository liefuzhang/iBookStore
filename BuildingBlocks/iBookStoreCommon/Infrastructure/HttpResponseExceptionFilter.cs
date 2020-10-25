using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iBookStoreCommon.Infrastructure
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        // The magic number 10 is subtracted from the maximum integer value.
        // Subtracting this number allows other filters to run at the very end of the pipeline.
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                context.Result = new ContentResult()
                {
                    Content = context.Exception.Message,
                    ContentType = "text/plain",
                    StatusCode = context.Exception is HttpResponseException ?
                        (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError
                };
                context.ExceptionHandled = true;
            }
        }
    }
}

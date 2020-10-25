using System;
using System.Net;

namespace iBookStoreCommon.Infrastructure
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException() { }

        public HttpResponseException(string message) : base(message) { }

        public HttpResponseException(string message, int status) : base(message)
        {
            Status = status;
        }

        public int Status { get; set; } = (int)HttpStatusCode.BadGateway;
    }
}

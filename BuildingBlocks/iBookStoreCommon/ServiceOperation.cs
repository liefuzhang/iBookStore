using System;
using System.Collections.Generic;
using System.Text;

namespace iBookStoreCommon
{
    public class ServiceOperation : IServiceOperation
    {
        public ServiceOperation(string httpMethod, string path)
        {
            HttpMethod = httpMethod;
            Path = path;
        }

        public string HttpMethod { get; }

        public string Path { get; }
    }
}

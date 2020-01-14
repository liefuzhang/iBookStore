using System;
using System.Collections.Generic;
using System.Text;
using iBookStoreCommon.ServiceRegistry;

namespace iBookStoreCommon.Dtos
{
    internal class ServiceOperationDto
    {
        public ServiceOperationDto(IServiceOperation operation)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            HttpMethod = operation.HttpMethod;
            Path = operation.Path;
        }

        public string HttpMethod { get; }

        public string Path { get; }
    }
}

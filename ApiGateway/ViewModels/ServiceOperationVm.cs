using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.ViewModels
{
    public class ServiceOperationVm
    {
        public string HttpMethod { get; set; }

        public string Path { get; set; }
    }
}

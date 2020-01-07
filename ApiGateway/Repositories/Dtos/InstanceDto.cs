using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Repositories.Dtos
{
    public class InstanceDto
    {
        public int ServiceInstanceId { get; set; }

        public int ServiceId { get; set; }

        public string Scheme { get; set; }
        
        public string IPAddress { get; set; }

        public string Port { get; set; }
    }
}

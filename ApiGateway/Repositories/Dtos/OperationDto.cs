using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Repositories.Dtos
{
    public class OperationDto
    {
        public int ServiceOperationId { get; set; }

        public int ServiceId { get; set; }

        public string HttpMethod { get; set; }

        public string Path { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class ApiGatewaySettings
    {
        /// <summary>
        /// The SQL Connection string for the ServiceRegistry database.
        /// </summary>
        public string ServiceRegistryConnectionString { get; set; }
    }
}

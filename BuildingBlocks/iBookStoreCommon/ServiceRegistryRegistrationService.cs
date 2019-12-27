using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Net.Http;

namespace iBookStoreCommon
{
    public class ServiceRegistryRegistrationService
    {
        private ServiceInstance _service;
        private readonly ServiceRegistryRepository _serviceRegistryRepository;

        public ServiceRegistryRegistrationService(ServiceRegistryRepository serviceRegistryRepository)
        {
            _serviceRegistryRepository = serviceRegistryRepository;
        }

        public async Task Initialize(IApplicationBuilder app, string appName)
        {
            var serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            var addressUri = new Uri(serverAddresses.Addresses.First());

            if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentNullException(nameof(appName));

            _service = new ServiceInstance
            {
                ServiceName = appName,
                Scheme = addressUri.Scheme,
                IpAddress = addressUri.Host,
                Port = addressUri.Port
            };

            _service.ServiceInstanceId = await _serviceRegistryRepository.RegisterService(_service);
        }
    }
}

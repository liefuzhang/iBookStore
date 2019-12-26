using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

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

        public async Task Initialize(IApplicationBuilder app)
        {
            _service = new ServiceInstance
            {
                ServiceName = "Test",
                Scheme = "test",
                IpAddress = "197.222.222.222",
                Port = 11111
            };

            _service.ServiceInstanceId = await _serviceRegistryRepository.RegisterService(_service);
        }
    }
}

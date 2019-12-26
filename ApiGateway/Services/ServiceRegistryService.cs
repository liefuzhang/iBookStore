using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.Models;
using ApiGateway.Repositories;

namespace ApiGateway.Services
{
    public class ServiceRegistryService : IServiceRegistryService
    {
        private readonly IServiceRegistryRepository _serviceRegistryRepository;

        public ServiceRegistryService(IServiceRegistryRepository serviceRegistryRepository)
        {
            _serviceRegistryRepository = serviceRegistryRepository;
        }

        public IService GetOrCreate(string serviceName)
        {
            return _serviceRegistryRepository.SelectService(serviceName)
                   ?? _serviceRegistryRepository.InsertService(serviceName);
        }

        public void RegisterInstance(IServiceInstance instance)
        {
            instance.EnsureInstanceIsValid();
            _serviceRegistryRepository.InsertInstance(instance);
        }
    }
}

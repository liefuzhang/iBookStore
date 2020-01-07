using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGateway.Models;
using ApiGateway.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.Services
{
    public interface IServiceOperationService
    {
        IServiceInstance GetServiceInstanceForRoute(RouteIdentifier route);
    }

    public class ServiceOperationService : IServiceOperationService
    {
        private static IEnumerable<IServiceOperation> _operations;
        private static IEnumerable<IServiceInstance> _instances;
        private IServiceRegistryRepository _registryRepository;

        public ServiceOperationService(IServiceRegistryRepository registryRepository)
        {
            _registryRepository = registryRepository;
            _operations = _registryRepository.SelectAllOperations();
            _instances = _registryRepository.SelectAllInstances();
        }

        public IServiceInstance GetServiceInstanceForRoute(RouteIdentifier route)
        {
            foreach (var operation in _operations)
            {
                if (operation.Route.TokenizedRouteEquals(route))
                {
                    return _instances.FirstOrDefault(i => i.Service.ServiceId == operation.Service.ServiceId);
                }
            }

            return null;
        }
    }
}

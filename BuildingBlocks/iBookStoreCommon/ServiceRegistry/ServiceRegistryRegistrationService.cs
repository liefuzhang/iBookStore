using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace iBookStoreCommon.ServiceRegistry
{
    public class ServiceRegistryRegistrationService
    {
        private ServiceInstance _service;
        private readonly ServiceRegistryRepository _serviceRegistryRepository;
        private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;

        public ServiceRegistryRegistrationService(ServiceRegistryRepository serviceRegistryRepository, IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            _serviceRegistryRepository = serviceRegistryRepository;
            _apiExplorer = apiExplorer;
        }

        public async Task Initialize(string appName, Uri appUri)
        {
            if (string.IsNullOrWhiteSpace(appName) || appUri == null)
                throw new ArgumentNullException(nameof(appName));

            _service = new ServiceInstance
            {
                ServiceName = appName,
                Scheme = appUri.Scheme,
                IpAddress = appUri.Host == "127.0.0.1" ? "localhost" : appUri.Host,
                Port = appUri.Port
            };

            _service.ServiceInstanceId = await _serviceRegistryRepository.RegisterService(_service);

            // Try to create ApiDescriptionServiceOperation object here so that we can make sure we have valid service operation. 
            var serviceOperations = _apiExplorer.ApiDescriptionGroups.Items
                .SelectMany(adg => adg.Items)
                .Select(ad => new ServiceOperation(ad.HttpMethod, ad.RelativePath))
                .ToList();

            if (serviceOperations.Any())
                await _serviceRegistryRepository.RegisterAllOperations(_service.ServiceName, serviceOperations);
        }
    }
}

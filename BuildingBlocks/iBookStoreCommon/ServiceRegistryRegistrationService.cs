using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Net.Http;
using iBookStoreCommon.Dtos;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace iBookStoreCommon
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

            // Try to create ApiDescriptionServiceOperation object here so that we can make sure we have valid service operation. 
            var serviceOperations = _apiExplorer.ApiDescriptionGroups.Items
                .SelectMany(adg => adg.Items)
                .Select(ad => new ServiceOperation(ad.HttpMethod, ad.RelativePath));

            await _serviceRegistryRepository.RegisterAllOperations(_service.ServiceName, serviceOperations);
        }
    }
}

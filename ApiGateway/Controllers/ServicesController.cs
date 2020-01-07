using System;
using System.Collections.Generic;
using ApiGateway.Models;
using ApiGateway.Services;
using ApiGateway.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ApiGateway.Controllers
{
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRegistryService _serviceRegistryService;

        public ServicesController(IServiceRegistryService serviceRegistryService)
        {
            _serviceRegistryService = serviceRegistryService;
        }

        /// <summary>
        /// Register a new instance of a service
        /// </summary>
        [HttpPost]
        [Route("/services/{serviceName}/instances")]
        public int RegisterInstance([FromRoute] string serviceName, [FromBody] ServiceInstanceEditVm instanceVm)
        {
            if (instanceVm == null) throw new Exception("ServiceInstance is required.");

            var service = _serviceRegistryService.GetOrCreate(serviceName);
            var instance = new ServiceInstance(instanceVm.Scheme, instanceVm.IpAddress, instanceVm.Port);
            service.AddInstance(instance);

            _serviceRegistryService.RegisterInstance(instance);

            return instance.ServiceInstanceId;
        }

        /// <summary>
        /// Add operations to a service
        /// </summary>
        [HttpPost]
        [Route("/services/{serviceName}/operations")]
        [Route("/services/{serviceName}/versions/{version}/operations")]
        public void CreateOperations([FromRoute] string serviceName, [FromBody] List<ServiceOperationVm> operations, [FromRoute] string version = "1.0.0")
        {
            if (operations == null || !operations.Any()) throw new Exception("At least one ServiceOperation must be supplied.");

            var service = _serviceRegistryService.GetOrCreate(serviceName);
            service.AddRangeOperation(operations.Select(o => new ServiceOperation(o.HttpMethod, o.Path)));

            _serviceRegistryService.ReplaceOperationsForService(service, version);
        }
    }
}

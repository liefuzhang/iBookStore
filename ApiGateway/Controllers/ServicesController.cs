using System;
using ApiGateway.Models;
using ApiGateway.Services;
using ApiGateway.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
    }
}

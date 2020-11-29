﻿using ApiGateway.Models;

namespace ApiGateway.Services
{
    public interface IServiceRegistryService
    {
        IService GetOrCreate(string serviceName);
        void RegisterInstance(IServiceInstance instance);
        void ReplaceOperationsForService(IService service, string version);
    }
}
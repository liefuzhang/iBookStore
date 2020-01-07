using System.Collections.Generic;
using ApiGateway.Models;

namespace ApiGateway.Repositories
{
    public interface IServiceRegistryRepository
    {
        IService SelectService(string serviceName);
        IService InsertService(string serviceName);
        void InsertInstance(IServiceInstance instance);
        List<IServiceOperation> SelectAllOperations();
        void DeleteAllOperations(int serviceId);
        void DeleteAllInstances(int serviceId);
        void BulkInsertServiceOperations(IEnumerable<IServiceOperation> serviceOperations);
        List<IServiceInstance> SelectAllInstances();
    }
}
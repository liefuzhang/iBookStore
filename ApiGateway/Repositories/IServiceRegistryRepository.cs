using ApiGateway.Models;

namespace ApiGateway.Repositories
{
    public interface IServiceRegistryRepository
    {
        IService SelectService(string serviceName);
        IService InsertService(string serviceName);
        void InsertInstance(IServiceInstance instance);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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

            _serviceRegistryRepository.DeleteAllInstances(instance.Service.ServiceId);
            _serviceRegistryRepository.InsertInstance(instance);
        }

        public void ReplaceOperationsForService(IService service, string version)
        {
            foreach (var operation in service.Operations)
            {
                operation.EnsureOperationIsValid();
            }

            using (var tx = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                // check for matching operation paths already registered
                var allOperations = _serviceRegistryRepository.SelectAllOperations();
                var errors = (
                    from newOperation in service.Operations
                    from existingOperation in allOperations
                    where newOperation.Service.ServiceId != existingOperation.Service.ServiceId
                    where existingOperation.TokenizedRouteEquals(newOperation)
                    select $"Operation {newOperation} is already in use by service {existingOperation.Service}.").ToList();
                
                if (errors.Any()) throw new Exception(string.Join(',', errors));

                // replace all operations for the given service
                _serviceRegistryRepository.DeleteAllOperations(service.ServiceId);

                _serviceRegistryRepository.BulkInsertServiceOperations(service.Operations);

                tx.Complete();
            }
        }
    }
}

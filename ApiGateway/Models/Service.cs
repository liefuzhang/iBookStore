using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Models
{
    public interface IService
    {
        int ServiceId { get; }

        string ServiceName { get; }
        void AddInstance(IServiceInstance instance);
        IEnumerable<IServiceOperation> Operations { get; }
        void AddRangeOperation(IEnumerable<ServiceOperation> operations);
    }

    public class Service : IService
    {
        public Service(string serviceName)
        {
            ServiceName = serviceName;
        }

        public Service(int serviceId, string serviceName) : this(serviceName)
        {
            ServiceId = serviceId;
        }

        private readonly List<IServiceInstance> _instances = new List<IServiceInstance>();

        private readonly List<IServiceOperation> _operations = new List<IServiceOperation>();

        public IEnumerable<IServiceInstance> Instances => _instances;

        public IEnumerable<IServiceOperation> Operations => _operations;

        public int ServiceId { get; }

        public string ServiceName { get; }

        public void AddRangeInstance(IEnumerable<ServiceInstance> instances)
        {
            foreach (var i in instances) AddInstance(i);
        }

        public void AddInstance(IServiceInstance instance)
        {
            instance.Service = this;
            _instances.Add(instance);
        }

        public void AddRangeOperation(IEnumerable<ServiceOperation> operations)
        {
            foreach (var o in operations) AddOperation(o);
        }

        private void AddOperation(ServiceOperation operation)
        {
            operation.Service = this;
            _operations.Add(operation);
        }
    }
}

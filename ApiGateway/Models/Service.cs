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

        public IEnumerable<IServiceInstance> Instances => _instances;

        public int ServiceId { get; }

        public string ServiceName { get; }

        public void AddInstance(IServiceInstance instance)
        {
            instance.Service = this;
            _instances.Add(instance);
        }
    }
}

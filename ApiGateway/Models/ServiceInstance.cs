using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Models
{
    public interface IServiceInstance
    {
        bool Disabled { get; }

        string IpAddress { get; }

        bool IsStatic { get; }

        string Port { get; }

        string Scheme { get; }

        Service Service { get; set; }

        int ServiceInstanceId { get; set; }
        void EnsureInstanceIsValid();
    }

    public class ServiceInstance : IServiceInstance
    {
        public ServiceInstance(string scheme, string ipAddress, string port, bool isStatic = false, bool disabled = false)
        {
            Scheme = scheme;
            IpAddress = ipAddress;
            Port = port;
            IsStatic = isStatic;
            Disabled = disabled;
        }

        public ServiceInstance(int serviceInstanceId, string scheme, string ipAddress, string port, bool isStatic = false, bool disabled = false)
            : this(scheme, ipAddress, port, isStatic, disabled)
        {
            ServiceInstanceId = serviceInstanceId;
        }


        private int? _serviceInstanceId;

        public int ServiceInstanceId
        {
            get => _serviceInstanceId.GetValueOrDefault();
            set
            {
                if (_serviceInstanceId.HasValue) throw new Exception("The ID for ServiceInstance cannot be changed");

                _serviceInstanceId = value;
            }
        }

        public Service Service { get; set; }

        public string Scheme { get; }

        public string IpAddress { get; }

        public string Port { get; }

        public bool IsStatic { get; }

        public bool Disabled { get; }

        public void EnsureInstanceIsValid()
        {
            if (string.IsNullOrWhiteSpace(Scheme)) throw new Exception($"{nameof(Scheme)} is required");
            if (string.IsNullOrWhiteSpace(IpAddress)) throw new Exception($"{nameof(IpAddress)} is required");
            if (string.IsNullOrWhiteSpace(Port)) throw new Exception($"{nameof(Port)} is required");
            if (string.IsNullOrWhiteSpace(Service.ServiceName)) throw new Exception($"{nameof(Service.ServiceName)} is required");
        }
    }
}

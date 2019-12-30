using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Models
{
    public interface IServiceOperation
    {
        RouteIdentifier Route { get; }

        IService Service { get; set; }

        int ServiceOperationId { get; set; }

        void EnsureOperationIsValid();

        bool TokenizedRouteEquals(IServiceOperation otherOperation);
    }

    public class ServiceOperation : IServiceOperation
    {
        public ServiceOperation(string httpMethod, string path)
        {
            Route = new RouteIdentifier(httpMethod, path);
        }

        private int? _serviceOperationId;

        public int ServiceOperationId
        {
            get => _serviceOperationId.GetValueOrDefault();
            set
            {
                if (_serviceOperationId.HasValue) throw new Exception("The ID for ServiceOperation cannot be changed");

                _serviceOperationId = value;
            }
        }

        public IService Service { get; set; }

        public RouteIdentifier Route { get; }

        public override string ToString()
        {
            var builder = string.Empty;

            if (ServiceOperationId != default(int)) builder += $"[{ServiceOperationId}] ";
            builder += Route.ToString();

            return builder;
        }

        public void EnsureOperationIsValid()
        {
            if (string.IsNullOrWhiteSpace(Route?.HttpMethod)) throw new Exception($"{nameof(Route.HttpMethod)} is required.");
            if (string.IsNullOrWhiteSpace(Route?.Path)) throw new Exception($"{nameof(Route.Path)} is required.");
        }


        public bool TokenizedRouteEquals(IServiceOperation otherOperation)
        {
            return Route.TokenizedRouteEquals(otherOperation.Route);
        }


        public static IEqualityComparer<IServiceOperation> RouteComparer => new OperationRouteComparer();


        private class OperationRouteComparer : IEqualityComparer<IServiceOperation>
        {
            private readonly IEqualityComparer<RouteIdentifier> _routeComparer = RouteIdentifier.TokenizedRouteEqualityComparer;

            public bool Equals(IServiceOperation a, IServiceOperation b)
            {
                return _routeComparer.Equals(a?.Route, b?.Route);
            }

            public int GetHashCode(IServiceOperation obj)
            {
                return _routeComparer.GetHashCode(obj.Route);
            }
        }
    }

}

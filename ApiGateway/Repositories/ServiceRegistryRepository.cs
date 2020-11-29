using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ApiGateway.Models;
using ApiGateway.Repositories.Dtos;
using Dapper;
using iBookStoreCommon.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using PostgreSQLCopyHelper;

namespace ApiGateway.Repositories
{
    public class ServiceRegistryRepository : IServiceRegistryRepository
    {
        private readonly ApiGatewaySettings _settings;
        private NpgsqlConnection Connection => new NpgsqlConnection(CommonHelper.GetConnectString(_settings.ConnectionString));

        public ServiceRegistryRepository(IOptions<ApiGatewaySettings> apiGatewaySettingsOptions)
        {
            _settings = apiGatewaySettingsOptions.Value;
        }

        public IService SelectService(string serviceName)
        {
            using (var conn = Connection)
            {
                var serviceDto = conn.QueryFirstOrDefault<ServiceDto>($"SELECT * FROM Services WHERE ServiceName = '{serviceName}'");
                return serviceDto == null ? null : new Service(serviceDto.ServiceId, serviceDto.ServiceName);
            }
        }

        public IService InsertService(string serviceName)
        {
            using (var conn = Connection)
            {
                var serviceId = conn.ExecuteScalar<int>($"INSERT INTO Services (ServiceName) VALUES ('{serviceName}'); " +
                                                        "SELECT lastval();");
                return new Service(serviceId, serviceName);
            }
        }

        public void InsertInstance(IServiceInstance instance)
        {
            var isStatic = instance.IsStatic ? "1" : "0";
            using (var conn = Connection)
                instance.ServiceInstanceId = conn.ExecuteScalar<int>(
                    $@"INSERT INTO ServiceInstances (
                            ServiceID, Scheme, IPAddress, Port, IsStatic
                          ) VALUES ('{instance.Service.ServiceId}', '{instance.Scheme}', '{instance.IpAddress}', '{instance.Port}', '{isStatic}')
                          ON CONFLICT (IPAddress, Port)
                          DO UPDATE SET
                            Scheme = EXCLUDED.Scheme,
                            IsStatic = EXCLUDED.IsStatic;
                        SELECT lastval();"
                    );
        }

        public List<IServiceInstance> SelectAllInstances()
        {
            using (var conn = Connection)
            {
                using (var dataset = conn.QueryMultiple(
                    "SELECT * FROM Services; " +
                    "SELECT * FROM ServiceInstances;"))
                {
                    var serviceDtos = dataset.Read<ServiceDto>();
                    var instanceDtos = dataset.Read<InstanceDto>();

                    return serviceDtos.Select(s =>
                        {
                            var service = new Service(s.ServiceId, s.ServiceName);

                            service.AddRangeInstance(instanceDtos
                                .Where(i => i.ServiceId == service.ServiceId)
                                .Select(i => new ServiceInstance(i.Scheme, i.IPAddress, i.Port)));

                            return service;
                        })
                        .SelectMany(s => s.Instances)
                        .ToList();
                }
            }
        }

        public List<IServiceOperation> SelectAllOperations()
        {
            using (var conn = Connection)
            {
                using (var dataset = conn.QueryMultiple(
                    "SELECT * FROM Services; " +
                    "SELECT * FROM ServiceOperations;"))
                {
                    var serviceDtos = dataset.Read<ServiceDto>();
                    var operationDtos = dataset.Read<OperationDto>();

                    return serviceDtos.Select(s =>
                        {
                            var service = new Service(s.ServiceId, s.ServiceName);

                            service.AddRangeOperation(operationDtos
                                .Where(o => o.ServiceId == service.ServiceId)
                                .Select(o => new ServiceOperation(o.HttpMethod, o.Path)));

                            return service;
                        })
                        .SelectMany(s => s.Operations)
                        .ToList();
                }
            }
        }

        public void DeleteAllOperations(int serviceId)
        {
            using (var conn = Connection)
                conn.Execute($"DELETE FROM ServiceOperations WHERE ServiceID = '{serviceId}'");
        }

        public void DeleteAllInstances(int serviceId)
        {
            using (var conn = Connection)
                conn.Execute($"DELETE FROM ServiceInstances WHERE ServiceID = '{serviceId}'");
        }

        public void BulkInsertServiceOperations(IEnumerable<IServiceOperation> serviceOperations)
        {
            using (var conn = Connection)
            {
                conn.Open();

                var copyHelper = new PostgreSQLCopyHelper<IServiceOperation>("ServiceOperations")
                    .MapInteger("ServiceID", x => x.Service.ServiceId)
                    .MapVarchar("HttpMethod", x => x.Route.HttpMethod)
                    .MapVarchar("Path", x => x.Route.Path);

                copyHelper.SaveAll(conn, serviceOperations);
            }
        }
    }
}

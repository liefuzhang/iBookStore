using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ApiGateway.Models;
using ApiGateway.Repositories.Dtos;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiGateway.Repositories
{
    public class ServiceRegistryRepository : IServiceRegistryRepository
    {
        private readonly ApiGatewaySettings _settings;
        private SqlConnection Connection => new SqlConnection(_settings.ServiceRegistryConnectionString);

        public ServiceRegistryRepository(IOptions<ApiGatewaySettings> apiGatewaySettingsOptions)
        {
            _settings = apiGatewaySettingsOptions.Value;
        }

        public IService SelectService(string serviceName)
        {
            using (var conn = Connection)
            {
                var serviceDto = conn.QueryFirstOrDefault<ServiceDto>("SELECT * FROM Services WHERE ServiceName = @ServiceName", new { serviceName });
                return serviceDto == null ? null : new Service(serviceDto.ServiceId, serviceDto.ServiceName);
            }
        }

        public IService InsertService(string serviceName)
        {
            using (var conn = Connection)
            {
                var serviceId = conn.ExecuteScalar<int>("INSERT INTO Services (ServiceName) VALUES (@ServiceName); " +
                                                        "SELECT SCOPE_IDENTITY();", new { serviceName });
                return new Service(serviceId, serviceName);
            }
        }

        public void InsertInstance(IServiceInstance instance)
        {
            using (var conn = Connection)
                instance.ServiceInstanceId = conn.ExecuteScalar<int>(
                    @"MERGE INTO ServiceInstances t
                    USING ( SELECT 
	                    ServiceID  = @ServiceID,
	                    Scheme     = @Scheme,
	                    IPAddress  = @IPAddress,
	                    [Port]     = @Port,
                        IsStatic   = @IsStatic
                    ) s
	                    ON s.IPAddress = t.IPAddress
	                    AND s.[Port] = t.[Port]
                    WHEN MATCHED THEN UPDATE SET 
	                    t.ServiceID = s.ServiceID,
	                    t.Scheme = s.Scheme
                    WHEN NOT MATCHED THEN
	                    INSERT ( ServiceID, Scheme, IPAddress, [Port], IsStatic )
	                    VALUES ( s.ServiceID, s.Scheme, s.IPAddress, s.[Port], s.IsStatic )
                    OUTPUT INSERTED.ServiceInstanceID
                    ;", new
                    {
                        instance.Service.ServiceId,
                        instance.Scheme,
                        instance.IpAddress,
                        instance.Port,
                        instance.IsStatic
                    });
        }

        public List<IServiceInstance> SelectAllInstances()
        {
            using (var conn = Connection)
            {
                using (var dataset = conn.QueryMultiple(
                    "SELECT * FROM Services; " +
                    "SELECT * FROM ServiceRegistry.ServiceInstances;"))
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
                    "SELECT * FROM ServiceRegistry.ServiceOperations;"))
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
                conn.Execute("DELETE FROM ServiceOperations WHERE ServiceID = @ServiceID", new { serviceId });
        }

        public void DeleteAllInstances(int serviceId)
        {
            using (var conn = Connection)
                conn.Execute("DELETE FROM ServiceInstances WHERE ServiceID = @ServiceID", new { serviceId });
        }

        public void BulkInsertServiceOperations(IEnumerable<IServiceOperation> serviceOperations)
        {
            using (var conn = Connection)
            using (var copy = new SqlBulkCopy(conn))
            {
                copy.DestinationTableName = "ServiceOperations";
                var table = new DataTable();
                table.Columns.Add("ServiceID", typeof(int));
                table.Columns.Add("HttpMethod", typeof(string));
                table.Columns.Add("Path", typeof(string));

                foreach (var column in table.Columns)
                {
                    copy.ColumnMappings.Add(column.ToString(), column.ToString());
                }

                foreach (var operation in serviceOperations)
                {
                    table.Rows.Add(operation.Service.ServiceId, operation.Route.HttpMethod, operation.Route.Path);
                }

                conn.Open();
                copy.WriteToServer(table);
            }
        }
    }
}

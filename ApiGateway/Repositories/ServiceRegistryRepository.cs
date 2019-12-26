using System.Data.SqlClient;
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
                var serviceDto = conn.QueryFirstOrDefault<ServiceDto>("SELECT * FROM ServiceRegistry.Services WHERE ServiceName = @ServiceName", new { serviceName });
                return serviceDto == null ? null : new Service(serviceDto.ServiceId, serviceDto.ServiceName);
            }
        }

        public IService InsertService(string serviceName)
        {
            using (var conn = Connection)
            {
                var serviceId = conn.ExecuteScalar<int>("INSERT INTO ServiceRegistry.Services (ServiceName) VALUES (@ServiceName); " +
                                                        "SELECT SCOPE_IDENTITY();", new { serviceName });
                return new Service(serviceId, serviceName);
            }
        }

        public void InsertInstance(IServiceInstance instance)
        {
            using (var conn = Connection)
                instance.ServiceInstanceId = conn.ExecuteScalar<int>(
                    @"MERGE INTO ServiceRegistry.ServiceInstances t
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
    }
}
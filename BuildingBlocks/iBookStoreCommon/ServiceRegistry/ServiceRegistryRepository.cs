﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using iBookStoreCommon.Dtos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace iBookStoreCommon.ServiceRegistry
{
    public class ServiceRegistryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceRegistryRepository> _logger;

        public ServiceRegistryRepository(ILogger<ServiceRegistryRepository> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<int?> RegisterService(ServiceInstance serviceInstance, string apiGatewayUrl)
        {
            try
            {
                var serviceContent = new StringContent(JsonConvert.SerializeObject(new
                {
                    serviceInstance.Scheme,
                    serviceInstance.IpAddress,
                    serviceInstance.Port,
                }), System.Text.Encoding.UTF8, "application/json");

                _logger.LogInformation($@"Register service: {apiGatewayUrl}/services/{serviceInstance.ServiceName}/instances");

                var response = await _httpClient.PostAsync($"{apiGatewayUrl}/services/{serviceInstance.ServiceName}/instances", serviceContent);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<int?>(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to register service: {serviceInstance.ServiceName} to the registry: {ex.Message}");
                return null;
            }
        }

        public async Task RegisterAllOperations(string serviceName, IEnumerable<IServiceOperation> serviceOperations,
            string apiGatewayUrl)
        {
            try
            {
                var serviceOperationDtos = serviceOperations.Select(o => new ServiceOperationDto(o));
                var content = new StringContent(JsonConvert.SerializeObject(serviceOperationDtos), System.Text.Encoding.UTF8, "application/json");
                var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

                await _httpClient.PostAsync($"{apiGatewayUrl}/services/{serviceName}/versions/{version ?? "1.0.0"}/operations", content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to register operations for service: {serviceName} to the registry: {ex.Message}");
                throw;
            }
        }

    }
}
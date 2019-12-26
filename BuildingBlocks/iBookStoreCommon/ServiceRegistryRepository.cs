using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace iBookStoreCommon
{
    public class ServiceRegistryRepository
    {
        private const string ServiceRegistry = "http://localhost:10340";
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceRegistryRepository> _logger;

        public ServiceRegistryRepository(ILogger<ServiceRegistryRepository> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<int?> RegisterService(ServiceInstance serviceInstance)
        {
            try
            {
                var serviceContent = new StringContent(JsonConvert.SerializeObject(new
                {
                    serviceInstance.Scheme,
                    serviceInstance.IpAddress,
                    serviceInstance.Port,
                }), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ServiceRegistry}/services/{serviceInstance.ServiceName}/instances", serviceContent);
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
    }
}
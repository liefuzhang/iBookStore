using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Recommendation.API.Models;

namespace Recommendation.API.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<AppSettings> _settings;

        public CatalogService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<List<CatalogItem>> GetCatalogItemsAsync(List<int> ids)
        {
            var url = $"{_settings.Value.ApiGatewayUrl}/api/catalog/multipleItems/" + string.Join(',', ids);

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(responseString);

            return catalogItems;
        }
    }
}

using Basket.API.Models;
using iBookStoreMVC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<AppSettings> _settings;

        public CatalogService(HttpClient httpClient, IOptions<AppSettings> settings) {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id) {
            var url = $"{_settings.Value.CatalogUrl}/api/v1/catalog/items/{id}";

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItems = JsonConvert.DeserializeObject<CatalogItem>(responseString);

            return catalogItems;
        }
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace iBookStoreMVC.Service
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/v1/catalog/";
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogTypes()
        {
            var url = API.Catalog.GetCatalogItems(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItems = JsonConvert.DeserializeObject<IEnumerable<CatalogItem>>(responseString);

            return catalogItems;
        }
    }
}
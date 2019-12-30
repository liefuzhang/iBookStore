using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ordering.API.Models;

namespace Ordering.API.Services
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
            var url = $"{_settings.Value.CatalogUrl}/api/catalog/items/{id}";

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItems = JsonConvert.DeserializeObject<CatalogItem>(responseString);

            return catalogItems;
        }
    }
}

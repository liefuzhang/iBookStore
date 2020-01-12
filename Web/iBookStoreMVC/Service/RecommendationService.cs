using System.Collections.Generic;
using System.Linq;
using iBookStoreMVC.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BasketService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public RecommendationService(HttpClient httpClient, ILogger<BasketService> logger, IOptions<AppSettings> settings) {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.ApiGatewayUrl}/api/recommendation";
        }

        public async Task<List<CatalogItem>> GetRecommendedBooks(int catalogItemId) {
            var url = API.Recommendation.GetRecommendedBooks(_remoteServiceBaseUrl, catalogItemId);

            var responseString = await _httpClient.GetStringAsync(url);

            var catalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(responseString);

            return catalogItems;
        }

        public async Task DeleteCatalogItem(int catalogItemId)
        {
            var url = API.Recommendation.DeleteCatalogItem(_remoteServiceBaseUrl, catalogItemId);

            var response = await _httpClient.DeleteAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}
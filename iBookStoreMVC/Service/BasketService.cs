using iBookStoreMVC.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BasketService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger, IOptions<AppSettings> settings) {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.BasketUrl}/api/v1/basket/";
        }

        public async Task AddItemToBasket(ApplicationUser user, int productId) {
            var url = API.Basket.AddItemToBasket(_remoteServiceBaseUrl);

            var newItem = new {
                CatalogItemId = productId,
                BasketId = user.Id,
                Quantity = 1
            };

            var basketContent = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(url, basketContent);
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var url = API.Basket.GetBasket(_remoteServiceBaseUrl, user.Id);

            var responseString = await _httpClient.GetStringAsync(url);

            return string.IsNullOrEmpty(responseString) ?
                new Basket() { BuyerId = user.Id } :
                JsonConvert.DeserializeObject<Basket>(responseString);
        }
    }
}
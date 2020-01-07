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
    public class WishlistService : IWishlistService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WishlistService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public WishlistService(HttpClient httpClient, ILogger<WishlistService> logger, IOptions<AppSettings> settings) {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.ApiGatewayUrl}/api/wishlist";
        }

        public async Task AddItemToWishlist(ApplicationUser user, int productId) {
            var url = API.Wishlist.AddItemToWishlist(_remoteServiceBaseUrl);

            var newItem = new {
                CatalogItemId = productId,
                WishlistId = user.Id
            };

            var wishlistContent = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(url, wishlistContent);
        }

        public async Task<Wishlist> GetWishlist(ApplicationUser user)
        {
            var url = API.Wishlist.GetWishlist(_remoteServiceBaseUrl, user.Id);

            var responseString = await _httpClient.GetStringAsync(url);

            return string.IsNullOrEmpty(responseString) ?
                new Wishlist { BuyerId = user.Id } :
                JsonConvert.DeserializeObject<Wishlist>(responseString);
        }

        public async Task DeleteItemFromWishlist(ApplicationUser user, string productId)
        {
            var url = API.Wishlist.DeleteItemFromWishlist(_remoteServiceBaseUrl, user.Id, productId);

            await _httpClient.DeleteAsync(url);
        }
    }
}
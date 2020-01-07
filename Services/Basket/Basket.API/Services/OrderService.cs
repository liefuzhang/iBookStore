using Basket.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<AppSettings> _settings;

        public OrderService(HttpClient httpClient, IOptions<AppSettings> settings) {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<OrderData> GetOrderDraftFromBasketAsync(CustomerBasket basket) {
            var url = $"{_settings.Value.ApiGatewayUrl}/api/order/draft";

            var content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OrderData>(ordersDraftResponse);
        }
    }
}

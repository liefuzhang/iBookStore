using Basket.API.Models;
using iBookStoreMVC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
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
            var url = $"{_settings.Value.OrderUrl}/api/v1/order/draft";

            var content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OrderData>(ordersDraftResponse);
        }
    }
}

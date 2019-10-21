using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ordering.BackgroundTasks.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BackgroundTaskSettings> _settings;

        public OrderService(HttpClient httpClient, IOptions<BackgroundTaskSettings> settings) {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task SetOrderAwaitingValidation(int orderId) {
            var url = $"{_settings.Value.OrderUrl}/api/v1/order/setOrderAwaitingValidation";

            var content = new StringContent(JsonConvert.SerializeObject(orderId), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.Models;
using iBookStoreMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace iBookStoreMVC.Service
{
    public class OrderingService : IOrderingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderingService> _logger;
        private readonly IOptions<AppSettings> _settings;
        private readonly string _remoteServiceBaseUrl;

        public OrderingService(HttpClient httpClient, ILogger<OrderingService> logger, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings;

            _remoteServiceBaseUrl = $"{_settings.Value.OrderUrl}/api/v1/order";
        }

        public async Task PlaceOrder(Order order) {
            var url = API.Order.PlaceOrder(_remoteServiceBaseUrl);
            var orderContent = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, orderContent);

            response.EnsureSuccessStatusCode();
        }

        public Order MapUserInfoIntoOrder(ApplicationUser user, Order order) {
            order.UserId = user.Id;
            order.UserName = user.FullName;

            order.City = user.City;
            order.Street = user.Street;
            order.State = user.State;
            order.Country = user.Country;
            order.ZipCode = user.ZipCode;

            order.CardNumber = user.CardNumber;
            order.CardHolderName = user.CardHolderName;
            order.CardExpiration = new DateTime(int.Parse("20" + user.Expiration.Split('/')[1]), int.Parse(user.Expiration.Split('/')[0]), 1);
            order.CardType = (CardType)user.CardType;

            return order;
        }

        public async Task<List<Order>> GetMyOrders() {
            var url = API.Order.GetAllMyOrders(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<List<Order>>(responseString);

            return response;
        }

        public async Task<List<Order>> GetAllOrders() {
            var url = API.Order.GetAllOrders(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<List<Order>>(responseString);

            return response;
        }

        public async Task CancelOrder(string orderId) {
            var order = new OrderCancelDTO() {
                OrderNumber = orderId
            };

            var url = API.Order.CancelOrder(_remoteServiceBaseUrl);
            var orderContent = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, orderContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                throw new Exception("Error cancelling order, try later.");
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<Order> GetOrder(string id) {
            var url = API.Order.GetOrder(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<Order>(responseString);

            return response;
        }

        public async Task ShipOrder(string id)
        {
            var url = API.Order.ShipOrder(_remoteServiceBaseUrl);
            var orderContent = new StringContent(JsonConvert.SerializeObject(id), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, orderContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                throw new Exception("Error shipping order, try later.");
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
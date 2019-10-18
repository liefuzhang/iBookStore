using System;
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

            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/v1/order/";
        }

        public Order MapUserInfoIntoOrder(ApplicationUser user, Order order) {
            order.City = user.City;
            order.Street = user.Street;
            order.State = user.State;
            order.Country = user.Country;
            order.ZipCode = user.ZipCode;

            order.CardNumber = user.CardNumber;
            order.CardHolderName = user.CardHolderName;
            order.CardExpiration = new DateTime(int.Parse("20" + user.Expiration.Split('/')[1]), int.Parse(user.Expiration.Split('/')[0]), 1);

            return order;
        }
    }
}
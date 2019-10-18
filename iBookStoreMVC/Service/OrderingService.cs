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

        public string MapUserInfoIntoOrder(ApplicationUser user, Order order) {
            throw new System.NotImplementedException();
        }
    }
}
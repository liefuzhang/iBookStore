﻿using Basket.API.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly IDistributedCache _cache;

        public BasketRepository(ILoggerFactory loggerFactory, IDistributedCache cache) {
            _logger = loggerFactory.CreateLogger<BasketRepository>();
            _cache = cache;
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId) {
            var data = await _cache.GetStringAsync(customerId);

            if (data == null) {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket) {
            await _cache.SetStringAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetBasketAsync(basket.BuyerId);
        }
    }
}

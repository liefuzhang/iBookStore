using Basket.API.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ILogger<WishlistRepository> _logger;
        private readonly ICacheService _cache;
        private const string WishlistCachePrefix = "Wishlist_";

        public WishlistRepository(ILoggerFactory loggerFactory, ICacheService cache)
        {
            _logger = loggerFactory.CreateLogger<WishlistRepository>();
            _cache = cache;
        }

        public async Task<CustomerWishlist> GetWishlistAsync(string customerId)
        {
            var data = await _cache.GetStringAsync(WishlistCachePrefix + customerId);

            if (data == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerWishlist>(data);
        }

        public async Task<CustomerWishlist> UpdateWishlistAsync(CustomerWishlist wishlist)
        {
            await _cache.SetStringAsync(WishlistCachePrefix + wishlist.BuyerId, JsonConvert.SerializeObject(wishlist));

            _logger.LogInformation("Wishlist item persisted succesfully.");

            return await GetWishlistAsync(wishlist.BuyerId);
        }
    }
}

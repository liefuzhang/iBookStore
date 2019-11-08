using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure
{
    public class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public Task RemoveAsync(string key) {
            if (_cache.ContainsKey(key))
                _cache.Remove(key, out string value);

            return Task.CompletedTask;
        }

        public Task SetStringAsync(string key, string value) {
            _cache.AddOrUpdate(key, value, (k, oldValue) => value);

            return Task.CompletedTask;
        }

        public Task<string> GetStringAsync(string key) {
            return Task.FromResult(_cache.GetValueOrDefault(key));
        }

        public Task<IEnumerable<string>> GetAllKeysAsync() {
            var keys = _cache.Keys.ToList();
            return Task.FromResult(keys.AsEnumerable());
        }
    }
}

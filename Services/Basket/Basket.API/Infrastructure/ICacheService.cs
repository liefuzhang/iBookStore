using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure
{
    public interface ICacheService
    {
        Task RemoveAsync(string key);
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
        Task<IEnumerable<string>> GetAllKeysAsync();
    }
}

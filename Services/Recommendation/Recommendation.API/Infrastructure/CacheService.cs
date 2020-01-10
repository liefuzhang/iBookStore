using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Recommendation.API.Infrastructure
{
    public class CacheService : ICacheService
    {
        // an cache entry looks like <Id1, [<Id2, count>, <Id3, count>]>
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, int>> _cache
            = new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>();
        private readonly IOptions<AppSettings> _settings;

        public CacheService(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        public Task UpdateBookRelations(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                if (!_cache.ContainsKey(id))
                {
                    _cache[id] = new ConcurrentDictionary<int, int>();
                }

                foreach (var otherId in ids)
                {
                    if (id == otherId)
                        continue;

                    if (_cache[id].ContainsKey(otherId))
                        _cache[id][otherId]++;
                    else
                        _cache[id][otherId] = 1;
                }
            }

            return Task.CompletedTask;
        }

        public Task<List<int>> GetRecommendedBookIdsForBook(int id)
        {
            if (!_cache.ContainsKey(id))
                return Task.FromResult(new List<int>());

            var relatedBooks = _cache[id];
            var amount = _settings.Value.AmountToRecommend;
            var books = relatedBooks.ToList()
                .OrderByDescending(b => b.Value)
                .Take(amount)
                .Select(b => b.Key)
                .ToList();

            return Task.FromResult(books);
        }
    }
}

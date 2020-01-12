using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recommendation.API.Infrastructure
{
    public interface ICacheService
    {
        Task UpdateBookRelations(IEnumerable<int> ids);
        Task<List<int>> GetRecommendedBookIdsForBook(int id);
        Task DeleteRecommendedBook(int bookId);
    }
}

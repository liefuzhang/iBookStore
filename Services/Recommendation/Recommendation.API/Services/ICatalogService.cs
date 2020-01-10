using System.Collections.Generic;
using System.Threading.Tasks;
using Recommendation.API.Models;

namespace Recommendation.API.Services
{
    public interface ICatalogService
    {
        Task<List<CatalogItem>> GetCatalogItemsAsync(List<int> ids);
    }
}
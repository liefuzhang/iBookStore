using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IRecommendationService
    {
        Task<List<CatalogItem>> GetRecommendedBooks(int catalogItemId);
        Task DeleteCatalogItem(int catalogItemId);
    }
}
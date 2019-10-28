using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogItem>> GetCatalogItems();
        Task<CatalogItem> GetCatalogItem(int catalogItemId);
    }
}
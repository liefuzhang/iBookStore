using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItems(int page, int take);
        Task<CatalogItem> GetCatalogItem(int catalogItemId);
    }
}
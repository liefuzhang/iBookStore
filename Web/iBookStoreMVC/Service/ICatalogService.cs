using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBookStoreMVC.Service
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItems(int page, int take, int? categoryFilterApplied, string searchTerm);
        Task<CatalogItem> GetCatalogItem(int catalogItemId);
        Task<IEnumerable<SelectListItem>> GetCategories();
        Task UpdateCatalogItem(CatalogItem catalogItem);
        Task DeleteCatalogItem(int catalogItemId);
    }
}
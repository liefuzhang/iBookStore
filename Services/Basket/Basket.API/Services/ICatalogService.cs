using Basket.API.Models;
using System.Threading.Tasks;

namespace Basket.API.Services
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);
    }
}
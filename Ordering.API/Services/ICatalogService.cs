using System.Threading.Tasks;
using Ordering.API.Models;

namespace Ordering.API.Services
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);
    }
}
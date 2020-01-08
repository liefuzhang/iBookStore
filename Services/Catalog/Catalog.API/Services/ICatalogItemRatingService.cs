using System.Threading.Tasks;
using Catalog.API.Models;

namespace Catalog.API.Services
{
    public interface ICatalogItemRatingService
    {
        Task<CatalogItemGoodreadRating> GetBookRatingFromGoodreads(string isbn);
    }
}
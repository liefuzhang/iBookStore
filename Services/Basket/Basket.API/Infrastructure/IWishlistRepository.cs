using System.Threading.Tasks;
using Basket.API.Models;

namespace Basket.API.Infrastructure
{
    public interface IWishlistRepository
    {
        Task<CustomerWishlist> GetWishlistAsync(string customerId);
        Task<CustomerWishlist> UpdateWishlistAsync(CustomerWishlist wishlist);
    }
}
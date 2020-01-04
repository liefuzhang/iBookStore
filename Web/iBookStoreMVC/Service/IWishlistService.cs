using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IWishlistService
    {
        Task AddItemToWishlist(ApplicationUser user, int productId);
        Task<Wishlist> GetWishlist(ApplicationUser user);
        Task DeleteItemFromWishlist(ApplicationUser user, string productId);
    }
}
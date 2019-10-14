using Identity.API.Models;
using System.Threading.Tasks;

namespace iBookStoreMVC.Service
{
    public interface IBasketService
    {
        Task AddItemToBasket(ApplicationUser user, int productId);
    }
}
using System.Threading.Tasks;
using Basket.API.Models;

namespace Basket.API.Services
{
    public interface IOrderService
    {
        Task<OrderData> GetOrderDraftFromBasketAsync(CustomerBasket basket);
    }
}
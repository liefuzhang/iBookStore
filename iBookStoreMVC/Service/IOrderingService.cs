using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IOrderingService
    {
        Order MapUserInfoIntoOrder(ApplicationUser user, Order order);
        Task PlaceOrder(Order order);
        Task<List<Order>> GetMyOrders();
        Task CancelOrder(string orderId);
        Task<Order> GetOrder(string orderId);
    }
}
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IOrderingService
    {
        string MapUserInfoIntoOrder(ApplicationUser user, Order order);
    }
}
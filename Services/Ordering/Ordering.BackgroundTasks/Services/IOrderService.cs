using System.Threading.Tasks;

namespace Ordering.BackgroundTasks.Services
{
    public interface IOrderService
    {
        Task SetOrderAwaitingValidation(int orderId);
    }
}
using Ordering.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<IEnumerable<OrderSummary>> GetOrdersFromForUserAsync(string userId);
    }
}
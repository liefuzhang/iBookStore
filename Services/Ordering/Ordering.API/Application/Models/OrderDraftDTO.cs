using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class OrderDraftDTO
    {
        public IEnumerable<OrderItemDTO> OrderItems { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }

        public static OrderDraftDTO FromOrder(Order order) {
            return new OrderDraftDTO() {
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO {
                    ProductId = oi.ProductId,
                    UnitPrice = oi.UnitPrice,
                    ISBN13 = oi.ISBN13,
                    Units = oi.Units,
                    ProductName = oi.ProductName
                }),
                Total = order.GetTotal(),
                Status = order.Status
            };
        }
    }
}

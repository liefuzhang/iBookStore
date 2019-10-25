using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class OrderDTO
    {
        public string OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Status { get; set; }

        public decimal Total { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardExpirationShort { get; set; }

        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public List<OrderItemDTO> OrderItems { get; } = new List<OrderItemDTO>();

        public Guid RequestId { get; set; }

        public static OrderDTO FromOrder(Order order) {
            var orderDTO = new OrderDTO {
                OrderNumber = order.Id.ToString(),
                CreatedDate = order.CreatedDate,
                Status = order.Status.ToString(),
                Total = order.Total,
                City = order.Address.City,
                Street = order.Address.Street,
                State = order.Address.State,
                Country = order.Address.Country,
                ZipCode = order.Address.ZipCode,
                CardNumber = order.CardNumber,
                CardExpiration = order.CardExpiration,
                CardExpirationShort = order.CardExpirationShort,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = order.CardTypeId,
                Buyer = order.Buyer
            };

            foreach(var item in order.OrderItems) {
                orderDTO.OrderItems.Add(new OrderItemDTO {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl,
                    UnitPrice = item.UnitPrice,
                    Units = item.Units
                });
            }

            return orderDTO;
        }
    }
}

using MediatR;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class CreateOrderCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
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

        public CardType CardType { get; set; }

        public string Buyer { get; set; }

        public List<OrderItemDTO> OrderItems { get; } = new List<OrderItemDTO>();

        public string Currency { get; set; }

        public decimal CurrencyRate { get; set; }

        public Guid RequestId { get; set; }
    }
}

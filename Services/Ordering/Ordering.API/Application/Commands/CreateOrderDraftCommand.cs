using MediatR;
using Ordering.API.Models;
using System.Collections.Generic;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderDraftCommand: IRequest<OrderDraftDTO>
    {
        public string BuyerId { get; private set; }

        public IEnumerable<BasketItem> Items { get; private set; }

        public string Currency { get; set; }

        public decimal CurrencyRate { get; set; }

        public CreateOrderDraftCommand(string buyerId, IEnumerable<BasketItem> items) {
            BuyerId = buyerId;
            Items = items;
        }
    }
}

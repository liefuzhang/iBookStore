using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.API.Extensions;
using Ordering.API.Models;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly OrderingContext _orderingContext;

        public CreateOrderCommandHandler(OrderingContext orderingContext) {
            _orderingContext = orderingContext;
        }

        public async Task<bool> Handle(CreateOrderCommand command, CancellationToken cancellationToken) {
            var buyer = await _orderingContext.Buyers.SingleOrDefaultAsync(b => b.IdentityGuid == command.UserId);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted) {
                buyer = new Buyer(command.UserId, command.UserName);
            }
            var paymentMethod = buyer.VerifyOrAddPaymentMethod(command.CardType,
                                           command.CardNumber,
                                           command.CardSecurityNumber,
                                           command.CardHolderName,
                                           command.CardExpiration);

            var buyerUpdated = buyerOriginallyExisted ?
                _orderingContext.Buyers.Update(buyer) :
                _orderingContext.Buyers.Add(buyer);

            await _orderingContext.SaveChangesAsync();

            var address = new Address(command.Street, command.City, command.State,
                command.Country, command.ZipCode);
            var order = new Order(command.UserId, command.UserName, address, (int)command.CardType, command.CardNumber,
                command.CardSecurityNumber, command.CardHolderName, command.CardExpiration);
            foreach (var item in command.OrderItems) {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice,
                    item.PictureUrl, item.Units);
            }

            _orderingContext.Orders.Add(order);
            await _orderingContext.SaveChangesAsync();

            return true;
        }
    }
}

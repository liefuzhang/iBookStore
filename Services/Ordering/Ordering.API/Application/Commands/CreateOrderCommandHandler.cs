using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.API.Extensions;
using Ordering.API.Models;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventBus;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Infrastructure;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly OrderingContext _orderingContext;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(OrderingContext orderingContext, IEventBus eventBus)
        {
            _orderingContext = orderingContext;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand command, CancellationToken cancellationToken) {
            // Add Integration event to clean the basket
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(command.UserId);
            _eventBus.Publish(orderStartedIntegrationEvent);

            var address = new Address(command.Street, command.City, command.State,
                command.Country, command.ZipCode);
            var order = new Order(command.UserId, command.UserName, address, (int)command.CardType, command.CardNumber,
                command.CardSecurityNumber, command.CardHolderName, command.CardExpiration);
            foreach (var item in command.OrderItems) {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice,
                    item.PictureUrl, item.Units);
            }

            _orderingContext.Orders.Add(order);
            await _orderingContext.SaveEntitiesAsync();

            return true;
        }
    }
}

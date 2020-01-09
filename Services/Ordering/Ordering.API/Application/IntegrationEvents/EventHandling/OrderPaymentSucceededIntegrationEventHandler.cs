using System;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.API.Application.Models;
using Ordering.API.Models;
using Ordering.Infrastructure;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class OrderPaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> _logger;
        private readonly OrderingContext _orderingContext;
        private readonly IEventBus _eventBus;

        public OrderPaymentSucceededIntegrationEventHandler(ILogger<OrderPaymentSucceededIntegrationEventHandler> logger, OrderingContext orderingContext, IEventBus eventBus) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingContext = orderingContext;
            _eventBus = eventBus;
        }

        public async Task Handle(OrderPaymentSucceededIntegrationEvent @event) {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            // Simulate a work time for setting paid status
            await Task.Delay(10 * 1000);

            var order = await _orderingContext.Orders
                .Include(o => o.OrderItems)
                .SingleAsync(o => o.Id == @event.OrderId);
            order.SetPaidStatus();
            await _orderingContext.SaveChangesAsync();
            
            var orderStockList = order.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));
            var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(order.Id, orderStockList);

            _eventBus.Publish(orderStatusChangedToPaidIntegrationEvent);

            await Task.CompletedTask;
        }
    }
}

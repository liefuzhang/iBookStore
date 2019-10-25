using System;
using System.Threading.Tasks;
using EventBus;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure;
using Ordering.API.IntegrationEvents.Events;

namespace Ordering.API.IntegrationEvents.EventHandling
{
    public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;
        private readonly OrderingContext _orderingContext;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger, OrderingContext orderingContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingContext = orderingContext;
        }

        public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var order = await _orderingContext.Orders.FindAsync(@event.OrderId);
            order?.SetCancelledStatus();
            await _orderingContext.SaveChangesAsync();
        }
    }
}

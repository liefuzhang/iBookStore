using System;
using System.Threading.Tasks;
using EventBus;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure;
using Ordering.API.IntegrationEvents.Events;

namespace Ordering.API.IntegrationEvents.EventHandling
{
    public class OrderPaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> _logger;
        private readonly OrderingContext _orderingContext;

        public OrderPaymentSucceededIntegrationEventHandler(ILogger<OrderPaymentSucceededIntegrationEventHandler> logger, OrderingContext orderingContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingContext = orderingContext;
        }

        public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            // Simulate a work time for setting paid status
            await Task.Delay(10 * 1000);

            var order = await _orderingContext.Orders.FindAsync(@event.OrderId);
            order?.SetPaidStatus();
            await _orderingContext.SaveChangesAsync();
        }
    }
}

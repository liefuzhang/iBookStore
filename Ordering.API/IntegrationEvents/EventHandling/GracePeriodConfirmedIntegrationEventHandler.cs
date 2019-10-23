using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Http;
using Ordering.API.Infrastructure;
using Ordering.API.IntegrationEvents.Events;

namespace Ordering.API.IntegrationEvents.EventHandling
{
    public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
    {
        private readonly OrderingContext _orderingContext;

        public GracePeriodConfirmedIntegrationEventHandler(OrderingContext orderingContext) {
            _orderingContext = orderingContext;
        }

        public async Task Handle(GracePeriodConfirmedIntegrationEvent @event) {
            var order = await _orderingContext.Orders.FindAsync(@event.OrderId);
            order?.SetAwaitingValidationStatus();
            await _orderingContext.SaveChangesAsync();
        }
    }
}

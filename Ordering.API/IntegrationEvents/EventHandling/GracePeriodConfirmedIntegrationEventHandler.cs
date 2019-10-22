using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Ordering.API.IntegrationEvents.Events;

namespace Ordering.API.IntegrationEvents.EventHandling
{
    public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
    {
        public Task Handle(GracePeriodConfirmedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}

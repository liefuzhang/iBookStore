using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;

namespace Ordering.API.IntegrationEvents.Events
{
    public class GracePeriodConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public GracePeriodConfirmedIntegrationEvent(int orderId) =>
            OrderId = orderId;
    }
}

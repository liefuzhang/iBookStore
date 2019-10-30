using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }

        public OrderStartedIntegrationEvent(string userId)
            => UserId = userId;
    }
}

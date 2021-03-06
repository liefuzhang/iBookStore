﻿using System.Collections.Generic;
using EventBus;
using Ordering.API.Application.Models;
using Ordering.API.Models;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }


        public OrderStatusChangedToPaidIntegrationEvent(int orderId, IEnumerable<OrderStockItem> orderStockItems)
        {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}

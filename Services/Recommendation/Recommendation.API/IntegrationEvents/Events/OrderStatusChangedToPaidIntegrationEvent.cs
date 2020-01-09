﻿using System.Collections.Generic;
using EventBus;
using Recommendation.API.Models;

namespace Recommendation.API.IntegrationEvents.Events
{
    public class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; }


        public OrderStatusChangedToPaidIntegrationEvent(int orderId, IEnumerable<OrderStockItem> orderStockItems) {
            OrderId = orderId;
            OrderStockItems = orderStockItems;
        }
    }
}

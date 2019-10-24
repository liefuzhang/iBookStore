using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Http;
using Ordering.API.Infrastructure;
using Ordering.API.IntegrationEvents.Events;
using Ordering.API.Models;
using Ordering.API.Services;

namespace Ordering.API.IntegrationEvents.EventHandling
{
    public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
    {
        private readonly OrderingContext _orderingContext;
        private readonly ICatalogService _catalogService;

        public GracePeriodConfirmedIntegrationEventHandler(OrderingContext orderingContext, ICatalogService catalogService) {
            _orderingContext = orderingContext;
            _catalogService = catalogService;
        }

        public async Task Handle(GracePeriodConfirmedIntegrationEvent @event) {
            var order = await _orderingContext.Orders.FindAsync(@event.OrderId);
            order?.SetAwaitingValidationStatus();
            await _orderingContext.SaveChangesAsync();

            // TODO move this job somewhere else
            await ValidateOrder(order);
        }

        private async Task ValidateOrder(Order order)
        {
            await Task.Delay(10 * 1000);
            if (await HasOrderItemsOnStock(order))
                order.SetStockConfirmedStatus();
            else
                order.SetStockRejectedStatus();

            await _orderingContext.SaveChangesAsync();
        }

        private async Task<bool> HasOrderItemsOnStock(Order order)
        {
            foreach (var orderItem in order.OrderItems) {
                var catalogItem = await _catalogService.GetCatalogItemAsync(orderItem.ProductId);
                if (catalogItem.AvailableStock < orderItem.Units)
                    return false;
            }

            return true;
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.Extensions.Logging;
using Recommendation.API.Infrastructure;
using Recommendation.API.IntegrationEvents.Events;

namespace Recommendation.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly ILogger<OrderStatusChangedToPaidIntegrationEventHandler> _logger;
        private readonly ICacheService _cacheService;

        public OrderStatusChangedToPaidIntegrationEventHandler(
            ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger,
            ICacheService cacheService)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _cacheService = cacheService;
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var productIds = @event.OrderStockItems.Select(i => i.ProductId).ToList();
            if (productIds.Count <= 1)
                return;

            await _cacheService.UpdateBookRelations(productIds);
        }
    }
}

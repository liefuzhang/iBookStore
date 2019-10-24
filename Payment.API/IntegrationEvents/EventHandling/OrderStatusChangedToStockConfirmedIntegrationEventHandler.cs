using System.Threading.Tasks;
using EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.API.IntegrationEvents.Events;

namespace Payment.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IOptions<AppSettings> _settings;
        private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(IEventBus eventBus,
            IOptions<AppSettings> settings,
            ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger) {
            _eventBus = eventBus;
            _settings = settings;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event) {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            await Task.Delay(10 * 1000);

            IntegrationEvent orderPaymentIntegrationEvent;
            if (_settings.Value.PaymentSucceeded)
                orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
            else
                orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);

            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, Program.AppName, orderPaymentIntegrationEvent);

            _eventBus.Publish(orderPaymentIntegrationEvent);

            await Task.CompletedTask;
        }
    }
}

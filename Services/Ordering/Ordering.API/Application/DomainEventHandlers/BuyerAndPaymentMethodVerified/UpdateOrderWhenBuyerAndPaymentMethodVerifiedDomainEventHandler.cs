using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ordering.Infrastructure;

namespace Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified
{
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
                   : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
    {
        private readonly OrderingContext _orderingContext;
        private readonly ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> _logger;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(OrderingContext orderingContext,
            ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> logger) {
            _orderingContext = orderingContext;
            _logger = logger;
        }

        // Domain Logic comment:
        // When the Buyer and Buyer's payment method have been created or verified that they existed, 
        // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent buyerPaymentMethodVerifiedEvent, CancellationToken cancellationToken) {
            var orderToUpdate = await _orderingContext.Orders.FindAsync(buyerPaymentMethodVerifiedEvent.OrderId);
            orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEvent.Buyer.Id);
            orderToUpdate.SetPaymentId(buyerPaymentMethodVerifiedEvent.Payment.Id);

            _logger.LogTrace("Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})",
                    buyerPaymentMethodVerifiedEvent.OrderId, nameof(buyerPaymentMethodVerifiedEvent.Payment), buyerPaymentMethodVerifiedEvent.Payment.Id);
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure;
using Ordering.API.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderStarted
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> _logger;
        private readonly OrderingContext _orderingContext;
        private readonly IIdentityService _identityService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
            ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger,
            OrderingContext orderingContext,
            IIdentityService identityService) {
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken) {
            var buyer = await _orderingContext.Buyers
                .Include(b => b.PaymentMethods)
                .SingleOrDefaultAsync(b => b.IdentityGuid == orderStartedEvent.UserId);
            var buyerOriginallyExisted = buyer != null;

            if (!buyerOriginallyExisted) {
                buyer = new Buyer(orderStartedEvent.UserId, orderStartedEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(orderStartedEvent.CardType,
                                           orderStartedEvent.CardNumber,
                                           orderStartedEvent.CardSecurityNumber,
                                           orderStartedEvent.CardHolderName,
                                           orderStartedEvent.CardExpiration,
                                           orderStartedEvent.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ?
                 _orderingContext.Buyers.Update(buyer) :
                _orderingContext.Buyers.Add(buyer);

            await _orderingContext.SaveEntitiesAsync();

            _logger.LogTrace("Buyer {BuyerId} and related payment method were validated or updated for orderId: {OrderId}.",
                    buyerUpdated.Entity.Id, orderStartedEvent.Order.Id);
        }
    }
}
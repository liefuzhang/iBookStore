﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.Infrastructure;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Models;
using EventBus;
using Microsoft.Extensions.Logging;
using Payment.API.IntegrationEvents.Events;

namespace Basket.API.IntegrationEvents.EventHandling
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;
        private readonly IBasketRepository _repository;

        public ProductPriceChangedIntegrationEventHandler(
            ILogger<ProductPriceChangedIntegrationEventHandler> logger,
            IBasketRepository repository) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(ProductPriceChangedIntegrationEvent @event) {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            var userIds = await _repository.GetAllBuyerIdsAsync();

            foreach (var id in userIds) {
                var basket = await _repository.GetBasketAsync(id);

                await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
            }
        }

        private async Task UpdatePriceInBasketItems(int productId, decimal newPrice, decimal oldPrice, CustomerBasket basket) {
            var itemsToUpdate = basket?.Items?.Where(x => x.ProductId == productId.ToString()).ToList();

            if (itemsToUpdate != null) {
                _logger.LogInformation("----- ProductPriceChangedIntegrationEventHandler - Updating items in basket for user: {BuyerId} ({@Items})", basket.BuyerId, itemsToUpdate);

                foreach (var item in itemsToUpdate) {
                    if (item.UnitPrice == oldPrice) {
                        item.UnitPrice = newPrice;
                    }
                }
                await _repository.UpdateBasketAsync(basket);
            }
        }
    }
}

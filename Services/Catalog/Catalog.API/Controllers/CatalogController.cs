﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.Models;
using Catalog.API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public CatalogController(CatalogContext catalogContext, ICatalogIntegrationEventService catalogIntegrationEventService) {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        // GET api/v1/[controller]/catalogItems[?pageIndex=0&pageSize=10]
        [HttpGet]
        [Route("catalogItems")]
        public async Task<IActionResult> CatalogItems([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/v1/[controller]/catalogItems/category/1[?pageIndex=0&pageSize=10]
        [HttpGet]
        [Route("catalogItems/category/{categoryId}")]
        public async Task<IActionResult> CatalogItemsByCategoryId(int categoryId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            var root = _catalogContext.CatalogItems.Where(ci => ci.CategoryId == categoryId);

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/v1/[controller]/items/{id}
        [HttpGet]
        [Route("items/{id}")]
        public async Task<CatalogItem> CatalogItem(int id) {
            return await _catalogContext.CatalogItems.FindAsync(id);
        }

        // GET api/v1/[controller]/categories
        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<List<Category>>> Categories() {
            return await _catalogContext.Categories.ToListAsync();
        }

        //PUT api/v1/[controller]/items
        [Route("items")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductAsync([FromBody]CatalogItem productToUpdate) {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null) {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }

            var oldPrice = catalogItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            // Update current product
            catalogItem.Price = productToUpdate.Price;

            if (raiseProductPriceChangedEvent) // Save product's data and publish integration event through the Event Bus if price has changed
            {
                //Create Integration Event to be published through the Event Bus
                var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                // Publish through the Event Bus and mark the saved event as published
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            } else // Just save the updated product because the Product's Price hasn't changed.
                await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CatalogItem), new { id = productToUpdate.Id }, null);
        }
    }
}
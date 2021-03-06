﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.Models;
using Catalog.API.Services;
using Catalog.API.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;
        private readonly ICatalogItemRatingService _catalogItemRatingService;

        public CatalogController(CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService,
            ICatalogItemRatingService catalogItemRatingService)
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
            _catalogItemRatingService = catalogItemRatingService;
        }

        // GET api/[controller]/catalogItems[?pageIndex=1&pageSize=10]
        [HttpGet]
        [Route("catalogItems")]
        public async Task<IActionResult> CatalogItems([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 1)
        {
            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();

            await PopulateBookRatings(itemsOnPage);

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/[controller]/catalogItems/category/1[?pageIndex=1&pageSize=10]
        [HttpGet]
        [Route("catalogItems/category/{categoryId}")]
        public async Task<IActionResult> CatalogItemsByCategoryId(int categoryId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 1)
        {
            var root = _catalogContext.CatalogItems.Where(ci => ci.CategoryId == categoryId);

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();

            await PopulateBookRatings(itemsOnPage);

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/[controller]/catalogItems/search/somebook[?pageIndex=1&pageSize=10]
        [HttpGet]
        [Route("catalogItems/search/{searchTerm}")]
        public async Task<IActionResult> CatalogItemsBySearchTerm(string searchTerm, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 1)
        {
            var root = _catalogContext.CatalogItems.Where(ci => ci.Name.ToLower().Contains(searchTerm.ToLower())
                                                                || ci.ISBN13 == searchTerm);
            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();

            await PopulateBookRatings(itemsOnPage);

            return Ok(new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }

        // GET api/[controller]/items/{id}
        [HttpGet]
        [Route("items/{id}")]
        public async Task<CatalogItem> CatalogItem(int id)
        {
            var item = await _catalogContext.CatalogItems.Include(c => c.Category)
                .SingleOrDefaultAsync(c => c.Id == id);
            item.Rating = await _catalogItemRatingService.GetBookRatingFromGoodreads(item.ISBN13);

            return item;
        }

        // GET api/[controller]/multipleItems/{ids}
        [HttpGet]
        [Route("multipleItems/{ids}")]
        public async Task<List<CatalogItem>> CatalogMultipleItems(string ids)
        {
            var idList = ids.Split(',').ToList();
            var items = await _catalogContext.CatalogItems.Include(c => c.Category)
                .Where(c => idList.Contains(c.Id.ToString()))
                .ToListAsync();
            await PopulateBookRatings(items);

            return items;
        }

        // GET api/[controller]/categories
        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<List<Category>>> Categories()
        {
            return await _catalogContext.Categories.ToListAsync();
        }

        // GET api/[controller]/bestSellers?top=10
        [HttpGet]
        [Route("bestSellers")]
        public async Task<ActionResult<List<CatalogItem>>> BestSellers([FromQuery]int top)
        {
            var catalogItems = await _catalogContext.CatalogItems
                .OrderByDescending(ci => ci.HistoricSaleCount)
                .Take(top)
                .ToListAsync();
            await PopulateBookRatings(catalogItems);

            return catalogItems;
        }

        // GET api/[controller]/newReleases?latest=10
        [HttpGet]
        [Route("newReleases")]
        public async Task<ActionResult<List<CatalogItem>>> NewReleases([FromQuery]int latest)
        {
            var catalogItems = await _catalogContext.CatalogItems
                .OrderByDescending(ci => ci.PublicationDate)
                .Take(latest)
                .ToListAsync();
            await PopulateBookRatings(catalogItems);

            return catalogItems;
        }

        //PUT api/[controller]/items
        [Route("items")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductAsync([FromBody]CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }

            var oldPrice = catalogItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            // Update current product
            catalogItem.Price = productToUpdate.Price;
            catalogItem.Name = productToUpdate.Name;
            catalogItem.Author = productToUpdate.Author;
            catalogItem.AvailableStock = productToUpdate.AvailableStock;
            catalogItem.Description = productToUpdate.Description;

            if (raiseProductPriceChangedEvent) // Save product's data and publish integration event through the Event Bus if price has changed
            {
                //Create Integration Event to be published through the Event Bus
                var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                // Publish through the Event Bus and mark the saved event as published
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            }
            else // Just save the updated product because the Product's Price hasn't changed.
                await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CatalogItem), new { id = productToUpdate.Id }, null);
        }

        // DELETE api/[controller]/items/{id}
        [HttpDelete]
        [Route("items/{id}")]
        public async Task<ActionResult> DeleteCatalogItem(int id)
        {
            var item = await _catalogContext.CatalogItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _catalogContext.CatalogItems.Remove(item);
            await _catalogContext.SaveChangesAsync();

            return Ok();
        }

        private async Task PopulateBookRatings(List<CatalogItem> items)
        {
            foreach (var item in items)
            {
                item.Rating = await _catalogItemRatingService.GetBookRatingFromGoodreads(item.ISBN13);
            }
        }
    }
}

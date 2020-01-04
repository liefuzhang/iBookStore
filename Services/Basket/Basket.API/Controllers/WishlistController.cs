﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.Infrastructure;
using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class WishlistController: ControllerBase
    {
        private readonly IWishlistRepository _repository;
        private readonly ICatalogService _catalogService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(
            ILogger<WishlistController> logger,
            IWishlistRepository repository,
            ICatalogService catalogService) {
            _logger = logger;
            _repository = repository;
            _catalogService = catalogService;
        }

        // POST api/v1/[controller]/items
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult> AddItemToWishlist([FromBody] AddWishlistItemRequest data) {
            if (data == null) {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            var item = await _catalogService.GetCatalogItemAsync(data.CatalogItemId);

            // Step 2: Get current wishlist status
            var currentWishlist = await _repository.GetWishlistAsync(data.WishlistId) ?? new CustomerWishlist(data.WishlistId);
            // Step 3: Merge current status with new product
            currentWishlist.Items.Add(new WishlistItem {
                UnitPrice = item.Price,
                ISBN13 = item.ISBN13,
                ProductId = item.Id.ToString(),
                ProductName = item.Name,
                Author = item.Author
            });

            // Step 4: Update wishlist
            await _repository.UpdateWishlistAsync(currentWishlist);

            return Ok();
        }

        // GET api/vi/[controller]/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<CustomerWishlist> GetWishlistByIdAsync(string id) {
            return await _repository.GetWishlistAsync(id) ?? new CustomerWishlist(id);
        }
    }
}
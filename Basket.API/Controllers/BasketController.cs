using System;
using System.Collections.Generic;
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
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityService _identityService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            ILogger<BasketController> logger,
            IBasketRepository repository,
            IIdentityService identityService) {
            _logger = logger;
            _repository = repository;
            _identityService = identityService;
        }

        // POST api/v1/[controller]/items
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult> AddItemToBasket([FromBody] AddBasketItemRequest data) {
            if (data == null || data.Quantity == 0) {
                return BadRequest("Invalid payload");
            }

            // Step 1: Get the item from catalog
            var item = await _catalogService.GetCatalogItemAsync(data.CatalogItemId);
            
            // Step 2: Get current basket status
            var currentBasket = (await _repository.GetBasketAsync(data.BasketId)) ?? new CustomerBasket(data.BasketId);
            // Step 3: Merge current status with new product
            currentBasket.Items.Add(new BasketItem() {
                UnitPrice = item.Price,
                PictureUrl = item.PictureUrl,
                ProductId = item.Id.ToString(),
                ProductName = item.Name,
                Quantity = data.Quantity,
                Id = Guid.NewGuid().ToString()
            });

            // Step 4: Update basket
            await _repository.UpdateBasketAsync(currentBasket);

            return Ok();
        }
    }
}

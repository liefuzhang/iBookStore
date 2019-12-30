using System;
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
        private readonly IOrderService _orderService;
        private readonly IIdentityService _identityService;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            ILogger<BasketController> logger,
            IBasketRepository repository,
            IIdentityService identityService,
            ICatalogService catalogService,
            IOrderService orderService) {
            _logger = logger;
            _repository = repository;
            _identityService = identityService;
            _catalogService = catalogService;
            _orderService = orderService;
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
            var currentBasket = await _repository.GetBasketAsync(data.BasketId) ?? new CustomerBasket(data.BasketId);
            // Step 3: Merge current status with new product
            currentBasket.Items.Add(new BasketItem() {
                UnitPrice = item.Price,
                ISBN13 = item.ISBN13,
                ProductId = item.Id.ToString(),
                ProductName = item.Name,
                Quantity = data.Quantity,
                Id = Guid.NewGuid().ToString()
            });

            // Step 4: Update basket
            await _repository.UpdateBasketAsync(currentBasket);

            return Ok();
        }

        // PUT api/v1/[controller]/items
        [HttpPut]
        [Route("items")]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] UpdateBasketItemsRequest data) {
            if (!data.Updates.Any()) {
                return BadRequest("No updates sent");
            }

            // Retrieve the current basket
            var currentBasket = await _repository.GetBasketAsync(data.BasketId);
            if (currentBasket == null) {
                return BadRequest($"Basket with id {data.BasketId} not found.");
            }

            // Update with new quantities
            foreach (var update in data.Updates) {
                var basketItem = currentBasket.Items.SingleOrDefault(item => item.Id == update.BasketItemId);
                if (basketItem == null) {
                    return BadRequest($"Basket item with id {update.BasketItemId} not found");
                }
                basketItem.Quantity = update.NewQuantity;
            }

            // Save the updated basket
            await _repository.UpdateBasketAsync(currentBasket);

            return currentBasket;
        }

        // GET api/vi/[controller]/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<CustomerBasket> GetBasketByIdAsync(string id) {
            return await _repository.GetBasketAsync(id) ?? new CustomerBasket(id);
        }

        [Route("orderDraft/{basketId}")]
        [HttpGet]
        public async Task<ActionResult<OrderData>> GetOrderDraftAsync(string basketId) {
            if (string.IsNullOrEmpty(basketId)) {
                return BadRequest("Need a valid basketid");
            }

            // Get the basket data and build a order draft based on it
            var currentBasket = await _repository.GetBasketAsync(basketId);
            if (currentBasket == null) {
                return BadRequest($"Basket with id {basketId} not found.");
            }

            return await _orderService.GetOrderDraftFromBasketAsync(currentBasket);
        }
    }
}
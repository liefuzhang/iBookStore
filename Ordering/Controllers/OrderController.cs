﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Extensions;
using Ordering.API.Infrastructure;
using Ordering.API.Models;
using Ordering.API.Services;

namespace Ordering.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly OrderingContext _orderingContext;

        public OrderController(IIdentityService identityService, OrderingContext orderingContext) {
            _identityService = identityService;
            _orderingContext = orderingContext;
        }

        // POST api/vi/[controller]/draft
        [HttpPost]
        [Route("draft")]
        public async Task<ActionResult<OrderDraftDTO>> CreateOrderDraftFromBasketDataAsync([FromBody] Basket basket) {
            var order = Order.NewDraft();
            var orderItems = basket.Items.Select(i => i.ToOrderItemDTO());
            foreach (var item in orderItems) {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.PictureUrl, item.Units);
            }

            return OrderDraftDTO.FromOrder(order);
        }

        // POST api/vi/[controller]/placeOrder
        [HttpPost]
        [Route("placeOrder")]
        public async Task PlaceOrder([FromBody] OrderDTO orderDTO) {
            var userId = _identityService.GetUserIdentity();
            var userName = User.FindFirst(x => x.Type == "unique_name").Value;


            _orderingContext.Orders.Add(Order.FromOrderDTO(orderDTO));
            await _orderingContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync() {
            var userid = _identityService.GetUserIdentity();
            var orders = _orderingContext.Orders.Select(o => new OrderSummary {
                CreatedDate = o.CreatedDate,
                OrderNumber = o.Id,
                Status = o.Status.ToString(),
                Total = o.Total
            });

            return Ok(orders);
        }
    }
}

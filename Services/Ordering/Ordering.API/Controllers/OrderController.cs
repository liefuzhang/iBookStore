using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.Commands;
using Ordering.API.Application.MediatRBehaviors;
using Ordering.API.Application.Queries;
using Ordering.API.Extensions;
using Ordering.API.Infrastructure;
using Ordering.API.Models;
using Ordering.API.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly IOrderQueries _orderQueries;
        private readonly OrderingContext _orderingContext;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IMediator mediator,
            IIdentityService identityService,
            IOrderQueries orderQueries,
            OrderingContext orderingContext,
            ILogger<OrderController> logger) {
            _mediator = mediator;
            _identityService = identityService;
            _orderQueries = orderQueries;
            _orderingContext = orderingContext;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST api/vi/[controller]/draft
        [HttpPost]
        [Route("draft")]
        public async Task<ActionResult<OrderDraftDTO>> CreateOrderDraftFromBasketDataAsync([FromBody] CreateOrderDraftCommand createOrderDraftCommand) {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createOrderDraftCommand.GetGenericTypeName(),
                nameof(createOrderDraftCommand.BuyerId),
                createOrderDraftCommand.BuyerId,
                createOrderDraftCommand);

            return await _mediator.Send(createOrderDraftCommand);
        }

        // POST api/vi/[controller]/placeOrder
        [HttpPost]
        [Route("placeOrder")]
        public async Task PlaceOrder([FromBody] OrderDTO orderDTO) {
            var buyer = await _orderingContext.Buyers.SingleOrDefaultAsync(b => b.IdentityGuid == orderDTO.UserId);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted) {
                buyer = new Buyer(orderDTO.UserId, orderDTO.UserName);
            }
            var paymentMethod = buyer.VerifyOrAddPaymentMethod(orderDTO.CardType,
                                           orderDTO.CardNumber,
                                           orderDTO.CardSecurityNumber,
                                           orderDTO.CardHolderName,
                                           orderDTO.CardExpiration);

            var buyerUpdated = buyerOriginallyExisted ?
                _orderingContext.Buyers.Update(buyer) :
                _orderingContext.Buyers.Add(buyer);

            await _orderingContext.SaveChangesAsync();

            var address = new Address(orderDTO.Street, orderDTO.City, orderDTO.State,
                orderDTO.Country, orderDTO.ZipCode);
            var order = new Order(address, buyerUpdated.Entity.Id, paymentMethod.Id);
            foreach (var item in orderDTO.OrderItems) {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice,
                    item.PictureUrl, item.Units);
            }

            _orderingContext.Orders.Add(order);
            await _orderingContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync() {
            var userId = _identityService.GetUserIdentity();
            var orders = await _orderQueries.GetOrdersFromForUserAsync(userId);

            return Ok(orders);
        }

        [HttpGet]
        [Route("allOrders")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetAllOrdersAsync() {
            var orders = _orderingContext.Orders.Select(o => new OrderSummary {
                CreatedDate = o.CreatedDate,
                OrderNumber = o.Id,
                Status = o.Status.ToString(),
                Total = o.GetTotal()
            });

            return Ok(orders);
        }

        // POST api/vi/[controller]/cancelOrder
        [HttpPost]
        [Route("cancelOrder")]
        public async Task CancelOrder([FromBody] OrderDTO orderDTO) {
            var order = _orderingContext.Orders.SingleOrDefault(o => o.Id.ToString() == orderDTO.OrderNumber);
            order?.SetCancelledStatus();
            await _orderingContext.SaveChangesAsync();
        }

        [Route("{orderId:int}")]
        [HttpGet]
        public async Task<ActionResult<OrderDTO>> GetOrderAsync(int orderId) {
            try {
                var order = await _orderingContext.Orders
                    .Include(o => o.OrderItems)
                    .SingleAsync(o => o.Id == orderId);

                return Ok(OrderDTO.FromOrder(order));
            } catch {
                return NotFound();
            }
        }

        // POST api/vi/[controller]/shipOrder
        [HttpPost]
        [Route("shipOrder")]
        public async Task ShipOrder([FromBody] string orderId) {
            var order = _orderingContext.Orders.SingleOrDefault(o => o.Id.ToString() == orderId);
            order?.SetShippedStatus();
            await _orderingContext.SaveChangesAsync();
        }
    }
}

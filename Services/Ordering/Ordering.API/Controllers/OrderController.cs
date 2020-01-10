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
using Ordering.API.Models;
using Ordering.API.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure;

namespace Ordering.Controllers
{
    [Route("api/[controller]")]
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
            ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _identityService = identityService;
            _orderQueries = orderQueries;
            _orderingContext = orderingContext;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST api/[controller]/draft
        [HttpPost]
        [Route("draft")]
        public async Task<ActionResult<OrderDraftDTO>> CreateOrderDraftFromBasketDataAsync([FromBody] CreateOrderDraftCommand createOrderDraftCommand)
        {
            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createOrderDraftCommand.GetGenericTypeName(),
                nameof(createOrderDraftCommand.BuyerId),
                createOrderDraftCommand.BuyerId,
                createOrderDraftCommand);

            return await _mediator.Send(createOrderDraftCommand);
        }

        // POST api/[controller]/placeOrder
        [HttpPost]
        [Route("placeOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderCommand command)
        {
            _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    command.GetGenericTypeName(),
                    nameof(command.OrderNumber),
                    command.OrderNumber,
                    command);

            var commandResult = await _mediator.Send(command);
            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync()
        {
            var userId = _identityService.GetUserIdentity();
            var orders = await _orderQueries.GetOrdersForUserAsync(userId);

            return Ok(orders);
        }

        [HttpGet]
        [Route("allOrders")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetAllOrdersAsync()
        {
            var orders = _orderingContext.Orders.Select(o => new OrderSummary
            {
                CreatedDate = o.CreatedDate,
                OrderNumber = o.Id,
                Status = o.Status,
                Total = o.GetTotal()
            });

            return Ok(orders);
        }

        // POST api/[controller]/cancelOrder
        [HttpPost]
        [Route("cancelOrder")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command)
        {
            _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    command.GetGenericTypeName(),
                    nameof(command.OrderNumber),
                    command.OrderNumber,
                    command);

            var commandResult = await _mediator.Send(command);
            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("{orderId:int}")]
        [HttpGet]
        public async Task<ActionResult<OrderDTO>> GetOrderAsync(int orderId)
        {
            try
            {
                var order = await _orderingContext.Orders
                    .Include(o => o.OrderItems)
                    .SingleAsync(o => o.Id == orderId);

                return Ok(OrderDTO.FromOrder(order));
            }
            catch
            {
                return NotFound();
            }
        }

        // POST api/[controller]/shipOrder
        [HttpPost]
        [Route("shipOrder")]
        public async Task<IActionResult> ShipOrder([FromBody] ShipOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, guid);

                _logger.LogInformation(
                    "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    requestShipOrder.GetGenericTypeName(),
                    nameof(requestShipOrder.Command.OrderNumber),
                    requestShipOrder.Command.OrderNumber,
                    requestShipOrder);

                commandResult = await _mediator.Send(requestShipOrder);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}

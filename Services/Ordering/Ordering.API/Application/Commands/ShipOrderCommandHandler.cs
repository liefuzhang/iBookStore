using MediatR;
using Ordering.API.Extensions;
using Ordering.API.Models;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ordering.Infrastructure;

namespace Ordering.API.Application.Commands
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, bool>
    {
        private readonly OrderingContext _orderingContext;

        public ShipOrderCommandHandler(OrderingContext orderingContext) {
            _orderingContext = orderingContext;
        }

        /// <summary>
        /// Handler which processes the command when
        /// administrator executes ship order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken) {
            var order = _orderingContext.Orders.SingleOrDefault(o => o.Id == command.OrderNumber);
            if (order == null)
                return false;

            order?.SetShippedStatus();
            await _orderingContext.SaveChangesAsync();

            return true;
        }
    }
}

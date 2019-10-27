using MediatR;
using Ordering.API.Extensions;
using Ordering.API.Infrastructure;
using Ordering.API.Models;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly OrderingContext _orderingContext;

        public CancelOrderCommandHandler(OrderingContext orderingContext) {
            _orderingContext = orderingContext;
        }

        /// <summary>
        /// Handler which processes the command when
        /// customer executes cancel order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken) {
            var order = _orderingContext.Orders.SingleOrDefault(o => o.Id == command.OrderNumber);
            if (order == null)
                return false;

            order?.SetCancelledStatus();
            await _orderingContext.SaveChangesAsync();

            return true;
        }
    }
}

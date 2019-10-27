using MediatR;
using Ordering.API.Models;
using System.Collections.Generic;

namespace Ordering.API.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public int OrderNumber { get; private set; }

        public CancelOrderCommand(int orderNumber) {
            OrderNumber = orderNumber;
        }
    }
}

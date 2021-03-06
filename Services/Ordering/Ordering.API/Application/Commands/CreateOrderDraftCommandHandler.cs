﻿using MediatR;
using Ordering.API.Extensions;
using Ordering.API.Models;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderDraftCommandHandler
        : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
    {
        public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand message, CancellationToken cancellationToken)
        {
            var order = Order.NewDraft();
            var orderItems = message.Items.Select(i => i.ToOrderItemDTO());
            foreach (var item in orderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.ISBN13, item.Units);
            }

            order.SetCurrency(message.Currency);
            order.SetCurrencyRate(message.CurrencyRate);

            return Task.FromResult(OrderDraftDTO.FromOrder(order));
        }
    }
}

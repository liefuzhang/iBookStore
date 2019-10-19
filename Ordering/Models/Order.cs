﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ordering.API.Models
{
    public class Order
    {
        public int Id { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Submitted;
        public decimal Total { get; private set; }
        public string City { get; private set; }

        public string Street { get; private set; }

        public string State { get; private set; }

        public string Country { get; private set; }

        public string ZipCode { get; private set; }

        public string CardNumber { get; private set; }

        public string CardHolderName { get; private set; }

        public bool IsDraft { get; private set; }

        public DateTime CardExpiration { get; private set; }

        public string CardExpirationShort { get; private set; }

        public string CardSecurityNumber { get; private set; }

        public int CardTypeId { get; private set; }

        public string Buyer { get; private set; }
        
        public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

        public static Order NewDraft() {
            var order = new Order();
            order.IsDraft = true;
            return order;
        }

        public static Order FromOrderDTO(OrderDTO orderDTO) {
            var order = new Order {
                CreatedDate = DateTime.Now,
                Total = orderDTO.Total,
                City = orderDTO.City,
                Street = orderDTO.Street,
                State = orderDTO.State,
                Country = orderDTO.Country,
                ZipCode = orderDTO.ZipCode,
                CardNumber = orderDTO.CardNumber,
                CardHolderName = orderDTO.CardHolderName,
                CardExpiration = orderDTO.CardExpiration,
                CardExpirationShort = orderDTO.CardExpirationShort,
                CardSecurityNumber = orderDTO.CardSecurityNumber,
                CardTypeId = orderDTO.CardTypeId,
                Buyer = orderDTO.Buyer
            };

            foreach (var item in orderDTO.OrderItems) {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.PictureUrl, item.Units);
            }

            return order;
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 
        public void AddOrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1) {
            var existingOrderForProduct = OrderItems.Where(o => o.ProductId == productId)
                .SingleOrDefault();

            if (existingOrderForProduct != null) {
                //if previous line exist modify it with higher discount and units..
                existingOrderForProduct.AddUnits(units);
            } else {
                //add validated new order item
                var orderItem = new OrderItem(productId, productName, unitPrice, pictureUrl, units);
                OrderItems.Add(orderItem);
            }
        }

        public decimal GetTotal() {
            return OrderItems.Sum(oi => oi.Units * oi.UnitPrice);
        }
    }
}
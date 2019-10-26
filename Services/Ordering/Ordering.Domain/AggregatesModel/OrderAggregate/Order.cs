using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order : Entity, IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Submitted;

        private bool _isDraft;

        public int? GetBuyerId => _buyerId;
        private int? _buyerId;

        public int? GetPaymentMethodId => _paymentMethodId;
        private int? _paymentMethodId;

        public Address Address { get; set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public static Order NewDraft() {
            var order = new Order();
            order._isDraft = true;
            return order;
        }

        protected Order() {
            _orderItems = new List<OrderItem>();
            _isDraft = false;
        }

        public Order(Address address, int? buyerId = null, int? paymentMethodId = null) : this() {
            Address = address;
            _buyerId = buyerId;
            _paymentMethodId = paymentMethodId;
            CreatedDate = DateTime.UtcNow;
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 
        public void AddOrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1) {
            var existingOrderForProduct = OrderItems
                .SingleOrDefault(o => o.ProductId == productId);

            if (existingOrderForProduct != null) {
                //if previous line exist modify it with higher discount and units..
                existingOrderForProduct.AddUnits(units);
            } else {
                //add validated new order item
                var orderItem = new OrderItem(productId, productName, unitPrice, pictureUrl, units);
                _orderItems.Add(orderItem);
            }
        }

        public decimal GetTotal() {
            return OrderItems.Sum(oi => oi.Units * oi.UnitPrice);
        }

        public void SetCancelledStatus() {
            if (Status == OrderStatus.Paid ||
                Status == OrderStatus.Shipped) {
                throw new Exception("Cannot change status to Cancelled");
            }

            Status = OrderStatus.Cancelled;
        }

        public void SetAwaitingValidationStatus() {
            if (Status == OrderStatus.Submitted) {
                Status = OrderStatus.AwaitingValidation;
            }
        }

        public void SetStockRejectedStatus() {
            if (Status == OrderStatus.AwaitingValidation) {
                Status = OrderStatus.StockRejected;
            }
        }

        public void SetStockConfirmedStatus() {
            if (Status == OrderStatus.AwaitingValidation) {
                Status = OrderStatus.StockConfirmed;
            }
        }

        public void SetPaidStatus() {
            if (Status == OrderStatus.StockConfirmed) {
                Status = OrderStatus.Paid;
            }
        }

        public void SetShippedStatus() {
            if (Status == OrderStatus.Paid) {
                Status = OrderStatus.Shipped;
            }
        }
    }
}
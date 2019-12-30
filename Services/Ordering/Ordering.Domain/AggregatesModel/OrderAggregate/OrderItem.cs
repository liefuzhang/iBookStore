using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class OrderItem : Entity
    {
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Units { get; private set; }
        public string ISBN13 { get; private set; }

        public int OrderId { get; set; }

        protected OrderItem()
        {
        }

        public OrderItem(int productId, string productName, decimal unitPrice, string isbn13, int units = 1) {
            if (units <= 0) {
                throw new Exception("Invalid number of units");
            }

            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Units = units;
            ISBN13 = isbn13;
        }

        public void AddUnits(int units) {
            if (units < 0) {
                throw new Exception("Invalid units");
            }

            Units += units;
        }
    }
}   
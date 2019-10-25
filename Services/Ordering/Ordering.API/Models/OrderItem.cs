using System;

namespace Ordering.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Units { get; private set; }
        public string PictureUrl { get; private set; }

        public int OrderId { get; set; }

        public OrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1) {
            if (units <= 0) {
                throw new Exception("Invalid number of units");
            }

            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Units = units;
            PictureUrl = pictureUrl;
        }

        public void AddUnits(int units) {
            if (units < 0) {
                throw new Exception("Invalid units");
            }

            Units += units;
        }
    }
}
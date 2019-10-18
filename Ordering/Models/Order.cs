using System;
using System.Collections.Generic;

namespace Ordering.API.Models
{
    public class Order
    {
        public string OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public bool IsDraft { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardExpirationShort { get; set; }
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public List<OrderItem> OrderItems { get; } = new List<OrderItem>();
    }
}
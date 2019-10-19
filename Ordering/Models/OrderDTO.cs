using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class OrderDTO
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

        public DateTime CardExpiration { get; set; }

        public string CardExpirationShort { get; set; }

        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public List<OrderItemDTO> OrderItems { get; } = new List<OrderItemDTO>();

        public Guid RequestId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class OrderSummary
    {
        public int OrderNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }
}

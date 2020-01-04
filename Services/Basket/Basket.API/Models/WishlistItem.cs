using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    public class WishlistItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Author { get; set; }
        public decimal UnitPrice { get; set; }
        public string ISBN13 { get; set; }
        public string Note { get; set; }
    }
}

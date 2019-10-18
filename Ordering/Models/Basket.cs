using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{
    public class Basket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; }

        public Basket(string customerId) {
            BuyerId = customerId;
            Items = new List<BasketItem>();
        }
    }
}

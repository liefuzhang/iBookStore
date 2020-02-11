using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    public class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyRate { get; set; }

        public CustomerBasket(string customerId) {
            BuyerId = customerId;
            Items = new List<BasketItem>();
        }
    }
}

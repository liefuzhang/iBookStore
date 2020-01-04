using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    public class CustomerWishlist
    {
        public string BuyerId { get; set; }
        public List<WishlistItem> Items { get; set; }

        public CustomerWishlist(string customerId) {
            BuyerId = customerId;
            Items = new List<WishlistItem>();
        }
    }
}

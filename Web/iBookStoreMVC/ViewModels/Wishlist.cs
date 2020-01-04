using System;
using System.Collections.Generic;
using System.Linq;

namespace iBookStoreMVC.ViewModels
{
    public class Wishlist
    {
        // Use property initializer syntax.
        // While this is often more useful for read only 
        // auto implemented properties, it can simplify logic
        // for read/write properties.
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public string BuyerId { get; set; }
    }
}
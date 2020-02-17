﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace iBookStoreMVC.ViewModels
{
    public class Basket
    {
        // Use property initializer syntax.
        // While this is often more useful for read only 
        // auto implemented properties, it can simplify logic
        // for read/write properties.
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public decimal Total() {
            return Math.Round(Items.Sum(i => i.ConvertedPrice * i.Quantity), 2);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace iBookStoreMVC.ViewModels
{
    public class WishlistItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Author { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ConvertedPrice { get; set; }
        public string ISBN13 { get; set; }
        public string Note { get; set; }
    }
}

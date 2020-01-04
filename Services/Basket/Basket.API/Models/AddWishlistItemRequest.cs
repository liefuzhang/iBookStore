using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    public class AddWishlistItemRequest
    {
        public int CatalogItemId { get; set; }
        public string WishlistId { get; set; }
    }
}

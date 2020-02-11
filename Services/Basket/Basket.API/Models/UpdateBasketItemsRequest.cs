using System.Collections.Generic;

namespace Basket.API.Models
{
    public class UpdateBasketItemsRequest
    {
        public string BasketId { get; set; }

        public ICollection<UpdateBasketItemData> Updates { get; set; }

        public string Currency { get; set; }

        public decimal CurrencyRate { get; set; }

        public UpdateBasketItemsRequest() {
            Updates = new List<UpdateBasketItemData>();
        }
    }
}
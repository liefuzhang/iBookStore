namespace Basket.API.Models
{
    public class UpdateBasketItemData
    {
        public string BasketItemId { get; set; }
        public int NewQuantity { get; set; }

        public UpdateBasketItemData() {
            NewQuantity = 0;
        }
    }
}
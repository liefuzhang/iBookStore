namespace Recommendation.API.Models
{
    public class OrderStockItem
    {
        public int ProductId { get; }
        public int Units { get; }

        public OrderStockItem(int productId, int units) {
            ProductId = productId;
            Units = units;
        }
    }
}

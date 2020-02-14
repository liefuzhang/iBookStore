namespace iBookStoreMVC.ViewModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal ConvertedPrice { get; set; }

        public int Units { get; set; }

        public string ISBN13 { get; set; }
    }
}
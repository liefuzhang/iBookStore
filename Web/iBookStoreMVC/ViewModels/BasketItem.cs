﻿namespace iBookStoreMVC.ViewModels
{
    public class BasketItem
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ConvertedPrice { get; set; }
        public int Quantity { get; set; }
        public string ISBN13 { get; set; }
    }
}
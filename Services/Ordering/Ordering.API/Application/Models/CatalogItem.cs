﻿namespace Ordering.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ISBN13 { get; set; }

        public int AvailableStock { get; set; }
    }
}
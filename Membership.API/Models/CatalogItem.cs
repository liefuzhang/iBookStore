﻿namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUri { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }
    }
}
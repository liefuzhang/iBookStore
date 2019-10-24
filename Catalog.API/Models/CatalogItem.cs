using System;

namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureUrl { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }

        // Quantity in stock
        public int AvailableStock { get; set; }

        public int RemoveStock(int quantityDesired)
        {
            if (AvailableStock == 0) {
                throw new Exception($"Empty stock, product item {Name} is sold out");
            }

            if (quantityDesired <= 0) {
                throw new Exception($"Item units desired should be greater than zero");
            }

            var removed = Math.Min(quantityDesired, this.AvailableStock);

            AvailableStock -= removed;

            return removed;
        }
    }
}
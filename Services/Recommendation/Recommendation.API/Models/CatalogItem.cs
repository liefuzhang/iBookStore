using System;

namespace Recommendation.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ISBN13 { get; set; }

        public int AvailableStock { get; set; }

        public CatalogItemGoodreadRating Rating { get; set; }

        public int TotalPage { get; set; }

        public DateTime PublicationDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.ViewModels
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal ConvertedPrice { get; set; }

        public string ISBN13 { get; set; }

        public CatalogItemGoodreadRating Rating { get; set; }

        public int TotalPage { get; set; }

        public DateTime PublicationDate { get; set; }

        public int AvailableStock { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }
    }
}

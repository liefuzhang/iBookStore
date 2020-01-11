using System.Collections.Generic;

namespace iBookStoreMVC.ViewModels
{
    public class CatalogItemDetailViewModel
    {
        public CatalogItem CatalogItem { get; set; }
        public IEnumerable<CatalogItem> RecommendedItems { get; set; }
    }
}
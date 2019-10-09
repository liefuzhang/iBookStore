using System.Collections.Generic;
using System.Linq;
using Catalog.API.Controllers;
using Catalog.API.Models;

namespace Catalog.API.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(CatalogContext context)
        {
            if (!context.CatalogItems.Any())
            {
                context.CatalogItems.AddRange(GetPreconfiguredCatalogItems());
            }

            context.SaveChanges();
        }

        private static IEnumerable<CatalogItem> GetPreconfiguredCatalogItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem { Name = "Week Light", Description = "Australia's bestselling cookbook author and most trusted home cook" },
                new CatalogItem { Name = "Sword of Kings", Description = "A fictional story" },
                new CatalogItem { Name = "Pride and Prejudice", Description = "An 1813 romantic novel of manners" }
            };
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Catalog.API.Controllers;
using Catalog.API.Models;

namespace Catalog.API.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(CatalogContext context) {
            if (!context.Categories.Any()) {
                context.Categories.AddRange(GetPreconfiguredCategories());
            }

            context.SaveChanges();

            var categoryLookup = context.Categories.ToDictionary(c => c.Name, c => c.Id);
            if (!context.CatalogItems.Any()) {
                context.CatalogItems.AddRange(GetPreconfiguredCatalogItems(categoryLookup));
            }

            context.SaveChanges();
        }

        private static IEnumerable<Category> GetPreconfiguredCategories() {
            return new List<Category>()
            {
                new Category { Name = "Classic" },
                new Category { Name = "Historic" },
                new Category { Name = "Novel" }
            };
        }

        private static IEnumerable<CatalogItem> GetPreconfiguredCatalogItems(Dictionary<string, int> categoryLookup) {
            return new List<CatalogItem>()
            {
                new CatalogItem { Name = "Week Light", Description = "Australia's bestselling cookbook author and most trusted home cook", CategoryId = categoryLookup["Classic"], Price = 22, Author = "Donna Hay", AvailableStock = 10 },
                new CatalogItem { Name = "Sword of Kings", Description = "A fictional story", CategoryId = categoryLookup["Novel"], Price = 18.7m, Author = "Bernard Cornwell", AvailableStock = 10 },
                new CatalogItem { Name = "Pride and Prejudice", Description = "An 1813 romantic novel of manners", CategoryId = categoryLookup["Novel"], Price = 11, Author = "Austin", AvailableStock = 10 }
            };
        }
    }
}
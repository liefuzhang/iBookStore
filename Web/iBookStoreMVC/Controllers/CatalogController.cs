using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using static System.Decimal;

namespace iBookStoreMVC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IRecommendationService _recommendationService;

        public CatalogController(ICatalogService catalogService, IRecommendationService recommendationService)
        {
            _catalogService = catalogService;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index(int? page, int? categoryFilterApplied, string searchTerm)
        {
            const int itemsPerPage = 12;
            var catalog = await _catalogService.GetCatalogItems(page ?? 1, itemsPerPage, categoryFilterApplied, searchTerm);

            if (HttpContext.Session.GetString("currencyRate") != null)
            {
                TryParse(HttpContext.Session.GetString("currencyRate"), out decimal rate);
                catalog.Data.ForEach(i => i.ConvertedPrice = i.Price * rate);
            }
            else
            {
                catalog.Data.ForEach(i => i.ConvertedPrice = i.Price);
            }

            var vm = new CatalogIndexViewModel()
            {
                CatalogItems = catalog.Data,
                Categories = await _catalogService.GetCategories(),
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 1,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count,
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPerPage))
                },
                CategoryFilterApplied = categoryFilterApplied,
                SearchTerm = searchTerm
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(int catalogItemId)
        {
            var catalogItem = await _catalogService.GetCatalogItem(catalogItemId);
            var recommendedItems = await _recommendationService.GetRecommendedBooks(catalogItemId);

            if (HttpContext.Session.GetString("currencyRate") != null)
            {
                TryParse(HttpContext.Session.GetString("currencyRate"), out decimal rate);
                catalogItem.ConvertedPrice = catalogItem.Price * rate;
            }
            else
            {
                catalogItem.ConvertedPrice = catalogItem.Price;
            }

            var vm = new CatalogItemDetailViewModel
            {
                CatalogItem = catalogItem,
                RecommendedItems = recommendedItems
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

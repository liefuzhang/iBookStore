using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index(int? page, int? categoryFilterApplied)
        {
            const int itemsPerPage = 12;
            var catalog = await _catalogService.GetCatalogItems(page ?? 0, itemsPerPage, categoryFilterApplied);

            var vm = new CatalogIndexViewModel() {
                CatalogItems = catalog.Data,
                Categories = await _catalogService.GetCategories(),
                PaginationInfo = new PaginationInfo() {
                    ActualPage = page ?? 0,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count,
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPerPage))
                },
                CategoryFilterApplied = categoryFilterApplied
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return View(vm);
        }

        public async Task<IActionResult> Detail(int catalogItemId) {
            var vm = await _catalogService.GetCatalogItem(catalogItemId);

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

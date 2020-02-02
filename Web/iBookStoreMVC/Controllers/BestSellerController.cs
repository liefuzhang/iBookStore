using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.Controllers
{
    public class BestSellerController : Controller
    {
        private readonly ICatalogService _catalogService;

        public BestSellerController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var topBestSellers = 20;
            var catalogItems = await _catalogService.GetBestSellers(topBestSellers);

            return View(catalogItems);
        }
    }
}

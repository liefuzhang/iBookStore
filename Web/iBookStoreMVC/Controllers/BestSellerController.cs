using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Decimal;

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

            if (HttpContext.Session.GetString("currencyRate") != null)
            {
                TryParse(HttpContext.Session.GetString("currencyRate"), out decimal rate);
                catalogItems.ForEach(i => i.ConvertedPrice = i.Price * rate);
            }
            else
            {
                catalogItems.ForEach(i => i.ConvertedPrice = i.Price);
            }

            return View(catalogItems);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.Controllers
{
    public class NewReleaseController : Controller
    {
        private readonly ICatalogService _catalogService;

        public NewReleaseController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            var latestNewReleases = 20;
            var catalogItems = await _catalogService.GetNewReleases(latestNewReleases);

            return View(catalogItems);
        }
    }
}

using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatalogManagementController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogManagementController(ICatalogService catalogSvc)
        {
            _catalogService = catalogSvc;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var itemsPerPage = 10;
            var vm = await _catalogService.GetCatalogItems(page ?? 0, itemsPerPage, null);
            return View(vm);
        }
    }
}
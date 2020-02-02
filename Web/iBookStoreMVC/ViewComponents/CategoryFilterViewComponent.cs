using System;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace iBookStoreMVC.ViewComponents
{
    public class CategoryFilterViewComponent : ViewComponent
    {
        private readonly ICatalogService _catalogService;

        public CategoryFilterViewComponent(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = await _catalogService.GetCategories();

            return View(vm);
        }
    }
}
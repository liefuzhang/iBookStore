﻿using System;
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
        private readonly IRecommendationService _recommendationService;

        public CatalogManagementController(ICatalogService catalogSvc, IRecommendationService recommendationService)
        {
            _catalogService = catalogSvc;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            const int itemsPerPage = 12;
            var catalog = await _catalogService.GetCatalogItems(page ?? 1, itemsPerPage, null, null);

            var vm = new CatalogManagementIndexViewModel()
            {
                CatalogItems = catalog.Data,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 1,
                    ItemsPerPage = catalog.Data.Count,
                    TotalItems = catalog.Count,
                    TotalPages = (int)Math.Ceiling(((decimal)catalog.Count / itemsPerPage))
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return View(vm);
        }

        public async Task<IActionResult> Detail(int catalogItemId)
        {
            var item = await _catalogService.GetCatalogItem(catalogItemId);

            return View(item);
        }

        public async Task<IActionResult> Update(CatalogItem catalogItem)
        {
            await _catalogService.UpdateCatalogItem(catalogItem);

            return RedirectToAction("Detail", new { catalogItem.Id });
        }

        public async Task<IActionResult> Delete(int catalogItemId, int page)
        {
            await _catalogService.DeleteCatalogItem(catalogItemId);
            await _recommendationService.DeleteCatalogItem(catalogItemId);

            return RedirectToAction("Index", new { page });
        }
    }
}
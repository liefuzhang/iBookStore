﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using Microsoft.AspNetCore.Authorization;
using iBookStoreMVC.ViewModels;
using Identity.API.Models;
using Microsoft.Extensions.Logging;

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly ILogger<CartController> _logger;

        public CartController(IBasketService basketSvc, ICatalogService catalogService, IIdentityParser<ApplicationUser> appUserParser,
            ILogger<CartController> logger) {
            _basketSvc = basketSvc;
            _catalogService = catalogService;
            _appUserParser = appUserParser;
            _logger = logger;
        }

        public async Task<IActionResult> AddToCart(CatalogItem item) {
            try {
                if (item?.Id != null) {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _basketSvc.AddItemToBasket(user, item.Id);
                }
                return RedirectToAction("Index", "Catalog");
            } catch (Exception e) {
                _logger.LogError(e, "Add to cart failed");
            }

            return RedirectToAction("Index", "Catalog", new { errorMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)" });
        }
    }
}
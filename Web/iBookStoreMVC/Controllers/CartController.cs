﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using Microsoft.AspNetCore.Authorization;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Decimal;

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
            ILogger<CartController> logger)
        {
            _basketSvc = basketSvc;
            _catalogService = catalogService;
            _appUserParser = appUserParser;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {
            try
            {
                var user = _appUserParser.Parse(HttpContext.User);
                decimal rate = 1;
                if (HttpContext.Session.GetString("currencyRate") != null)
                {
                    TryParse(HttpContext.Session.GetString("currencyRate"), out rate);
                }

                await _basketSvc.SetQuantities(user, quantities, HttpContext.Session.GetString("currency") ?? "NZD", rate);
                if (action == "Checkout")
                {
                    return RedirectToAction("Create", "Order");
                }
            }
            catch (BrokenCircuitException e)
            {
                // Catch error when Basket.api is in circuit-opened mode                 
                ViewBag.BasketInoperativeMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
            }

            return View();
        }

        public async Task<IActionResult> AddToCart(int itemId)
        {
            try
            {
                if (itemId > 0)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _basketSvc.AddItemToBasket(user, itemId);
                }
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Add to cart failed");
            }

            return RedirectToAction("Index", "Catalog", new { errorMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)" });
        }

    }
}

using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Decimal;

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistSvc;
        private readonly IBasketService _basketSvc;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly ILogger<CartController> _logger;

        public WishlistController(IWishlistService wishlistSvc,
            IBasketService basketSvc,
            ICatalogService catalogService,
            IIdentityParser<ApplicationUser> appUserParser,
            ILogger<CartController> logger)
        {
            _wishlistSvc = wishlistSvc;
            _catalogService = catalogService;
            _appUserParser = appUserParser;
            _logger = logger;
            _basketSvc = basketSvc;
        }

        public async Task<IActionResult> Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var wishlist = await _wishlistSvc.GetWishlist(user);

            if (HttpContext.Session.GetString("currencyRate") != null)
            {
                TryParse(HttpContext.Session.GetString("currencyRate"), out decimal rate);
                wishlist.Items.ForEach(i => i.ConvertedPrice = i.UnitPrice * rate);
            }
            else
            {
                wishlist.Items.ForEach(i => i.ConvertedPrice = i.UnitPrice);
            }

            return View(wishlist.Items);
        }

        public async Task<IActionResult> AddToWishlist(CatalogItem item)
        {
            try
            {
                if (item?.Id != null)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _wishlistSvc.AddItemToWishlist(user, item.Id);
                }
                return RedirectToAction("Index", "Wishlist");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Add to wishlist failed");
            }

            return RedirectToAction("Index", "Wishlist", new { errorMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)" });
        }

        public async Task<IActionResult> AddToCart(string productId)
        {
            try
            {
                if (productId != null)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _basketSvc.AddItemToBasket(user, int.Parse(productId));
                    await DeleteItem(productId);
                }
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Add to cart failed");
            }

            return RedirectToAction("Index", "Wishlist", new { errorMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)" });
        }

        public async Task<IActionResult> DeleteItem(string productId)
        {
            try
            {
                if (productId != null)
                {
                    var user = _appUserParser.Parse(HttpContext.User);
                    await _wishlistSvc.DeleteItemFromWishlist(user, productId);
                }
                return RedirectToAction("Index", "Wishlist");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Delete from wishlist failed");
            }

            return RedirectToAction("Index", "Wishlist", new { errorMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)" });
        }
    }
}

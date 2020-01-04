using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistSvc;
        private readonly ICatalogService _catalogService;
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly ILogger<CartController> _logger;

        public WishlistController(IWishlistService wishlistSvc, ICatalogService catalogService, IIdentityParser<ApplicationUser> appUserParser,
            ILogger<CartController> logger)
        {
            _wishlistSvc = wishlistSvc;
            _catalogService = catalogService;
            _appUserParser = appUserParser;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var wishlist = await _wishlistSvc.GetWishlist(user);
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

        public IActionResult AddToCart()
        {
            
        }
    }
}

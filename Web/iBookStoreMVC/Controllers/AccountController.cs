using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public AccountController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<IActionResult> SignIn(string returnUrl)
        {
            var user = User as ClaimsPrincipal;

            var token = await HttpContext.GetTokenAsync("access_token");

            if (token != null)
            {
                ViewData["access_token"] = token;
            }

            return RedirectToAction(nameof(CatalogController.Index), "Catalog");
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // "Catalog" because UrlHelper doesn't support nameof() for controllers
            // https://github.com/aspnet/Mvc/issues/5853
            var homeUrl = Url.Action(nameof(CatalogController.Index), "Catalog");
            return new SignOutResult(OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = homeUrl });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ChangeCurrency(string currency)
        {
            var rate = await _currencyService.GetCurrencyRate(currency);
            if (rate != null)
            {
                HttpContext.Session.SetString("currency", currency);
                HttpContext.Session.SetString("currencyRate", rate.ToString());
            }

            return RedirectToAction(nameof(CatalogController.Index), "Catalog");
        }
    }
}
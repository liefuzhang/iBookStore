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

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [Authorize]
        public async Task<IActionResult> SignIn(string returnUrl) {
            var user = User as ClaimsPrincipal;

            var token = await HttpContext.GetTokenAsync("access_token");

            if (token != null) {
                ViewData["access_token"] = token;
            }

            return RedirectToAction(nameof(CatalogController.Index), "Catalog");
        }
    }
}

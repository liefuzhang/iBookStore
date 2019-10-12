using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Models.AccountViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public AccountController(IIdentityServerInteractionService interaction,
            IClientStore clientStore) {
            _interaction = interaction;
            _clientStore = clientStore;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl) {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null) {
                throw new NotImplementedException("External login is not implemented!");
            }

            var vm = new LoginViewModel {
                ReturnUrl = returnUrl,
                Email = context?.LoginHint,
            };

            ViewData["ReturnUrl"] = returnUrl;

            return View(vm);
        }
    }
}
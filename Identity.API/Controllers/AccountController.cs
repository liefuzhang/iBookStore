using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.API.Models;
using Identity.API.Models.AccountViewModels;
using Identity.API.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILoginService<ApplicationUser> _loginService;
        private readonly IClientStore _clientStore;

        public AccountController(IIdentityServerInteractionService interaction,
            IClientStore clientStore, ILoginService<ApplicationUser> loginService) {
            _interaction = interaction;
            _clientStore = clientStore;
            _loginService = loginService;
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

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var user = await _loginService.FindByUsername(model.Email);

                if (await _loginService.ValidateCredentials(user, model.Password)) {
                    var props = new AuthenticationProperties {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                        AllowRefresh = true,
                        RedirectUri = model.ReturnUrl
                    };

                    if (model.RememberMe) {
                        props = new AuthenticationProperties {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddYears(10)
                        };
                    };

                    await _loginService.SignInAsync(user, props);

                    // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint
                    if (_interaction.IsValidReturnUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // something went wrong, show form with error
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            var vm = new LoginViewModel {
                ReturnUrl = model.ReturnUrl,
                Email = context?.LoginHint,
            };
            vm.Email = model.Email;
            vm.RememberMe = model.RememberMe;

            ViewData["ReturnUrl"] = model.ReturnUrl;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId) {
            var model = new LogoutViewModel { LogoutId = logoutId };
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider) {
                if (model.LogoutId == null) {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;

                await HttpContext.SignOutAsync(idp, new AuthenticationProperties {
                    RedirectUri = url
                });
            }

            // delete authentication cookie
            await HttpContext.SignOutAsync();

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);
            return Redirect(logout?.PostLogoutRedirectUri);
        }
    }
}
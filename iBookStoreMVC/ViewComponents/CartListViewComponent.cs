using System;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.ViewComponents
{
    public class CartListViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public CartListViewComponent(IBasketService basketService) => _basketService = basketService;

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user) {
            var vm = new Basket();
            try {
                vm = await GetCartAsync(user);
                return View(vm);
            } catch (/*BrokenCircuitException*/ Exception e) {
                // Catch error when Basket.api is in circuit-opened mode                 
                ViewBag.BasketInoperativeMsg = "Basket Service is inoperative, please try later on. (Business Msg Due to Circuit-Breaker)";
            }

            return View(vm);
        }

        private Task<Basket> GetCartAsync(ApplicationUser user) => _basketService.GetBasket(user);
    }
}
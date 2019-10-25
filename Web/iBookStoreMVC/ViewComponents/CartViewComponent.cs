using System;
using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public CartViewComponent(IBasketService basketService) => _basketService = basketService;

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user) {
            var vm = new CartComponentViewModel();
            try {
                var itemsCountInCart = await ItemsCountInCartAsync(user);
                vm.ItemsCount = itemsCountInCart;
                return View(vm);
            } catch (/*BrokenCircuitException*/Exception e) {
                // Catch error when Basket.api is in circuit-opened mode                 
                ViewBag.IsBasketInoperative = true;
            }

            return View(vm);
        }
        private async Task<int> ItemsCountInCartAsync(ApplicationUser user) {
            var basket = await _basketService.GetBasket(user);
            return basket.Items.Count;
        }
    }
}
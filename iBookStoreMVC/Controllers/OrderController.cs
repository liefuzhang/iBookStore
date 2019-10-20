using System;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private IBasketService _basketSvc;
        private IOrderingService _orderSvc;

        public OrderController(IIdentityParser<ApplicationUser> appUserParser, IBasketService basketSvc, IOrderingService orderSvc)
        {
            _appUserParser = appUserParser;
            _basketSvc = basketSvc;
            _orderSvc = orderSvc;
        }

        public async Task<IActionResult> Create()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            var order = await _basketSvc.GetOrderDraft(user.Id);
            var vm = _orderSvc.MapUserInfoIntoOrder(user, order);
            vm.CardExpirationShortFormat();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order) {
            try {
                if (ModelState.IsValid) {
                    var user = _appUserParser.Parse(HttpContext.User);

                    await _orderSvc.PlaceOrder(order);
                    await _basketSvc.ClearBasket(user.Id);

                    //Redirect to historic list.
                    return RedirectToAction("Index");
                }
            } catch (Exception) {
                ModelState.AddModelError("Error", "It was not possible to create a new order, please try later on. (Business Msg Due to Circuit-Breaker)");
            }

            return View("Create", order);
        }

        public async Task<IActionResult> Index(Order item) {
            var vm = await _orderSvc.GetMyOrders();
            return View(vm);
        }

        public async Task<IActionResult> Cancel(string orderId) {
            await _orderSvc.CancelOrder(orderId);

            //Redirect to historic list.
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(string orderId) {
            var user = _appUserParser.Parse(HttpContext.User);

            var order = await _orderSvc.GetOrder(orderId);
            return View(order);
        }
    }
}
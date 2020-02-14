using System;
using System.Threading.Tasks;
using iBookStoreMVC.Infrastructure;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace iBookStoreMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _appUserParser;
        private readonly IBasketService _basketSvc;
        private readonly IOrderingService _orderSvc;

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
            order.OrderItems.ForEach(i => i.ConvertedPrice = i.UnitPrice * order.CurrencyRate);

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

                    //Redirect to historic list.
                    return RedirectToAction("Index");
                }
            } catch (BrokenCircuitException) {
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
            var order = await _orderSvc.GetOrder(orderId);
            order.OrderItems.ForEach(i => i.ConvertedPrice = i.UnitPrice * order.CurrencyRate);

            return View(order);
        }
    }
}
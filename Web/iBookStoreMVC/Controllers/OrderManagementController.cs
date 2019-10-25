using System.Threading.Tasks;
using iBookStoreMVC.Service;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderingService _orderSvc;

        public OrderManagementController(IOrderingService orderSvc)
        {
            _orderSvc = orderSvc;
        }

        public async Task<IActionResult> Index(Order item) {
            var vm = await _orderSvc.GetAllOrders();
            return View(vm);
        }

        public async Task<IActionResult> OrderProcess(string orderId)
        {
            await _orderSvc.ShipOrder(orderId);

            return RedirectToAction("Index");
        }
    }
}
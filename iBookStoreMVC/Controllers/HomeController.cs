using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;

namespace iBookStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMembershipService _membershipService;

        public HomeController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _membershipService.GetMembershipTypes();

            return View(vm);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

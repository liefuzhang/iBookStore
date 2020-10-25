using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Models;
using UserManagement.API.Service;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly INewsletterService _newsletterService;

        public UserManagementController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [Route("signUpNewsletter")]
        [HttpPost]
        public void SignUpNewsletter([FromBody] SignUpNewsletterVm vm)
        {
            _newsletterService.SignUpNewsletter(vm.Email);
        }
    }
}
 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iBookStoreMVC.Models;
using iBookStoreMVC.Service;
using Microsoft.AspNetCore.Authorization;
using iBookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Decimal;

namespace iBookStoreMVC.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IUserManagementService userManagementService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task SignUpNewsletter(string email)
        {
            try
            {
                await _userManagementService.SignUpNewsletter(email);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Sign up newsletter failed");
            }
        }
    }
}

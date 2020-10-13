using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using UserManagement.API.Infrastructure;
using UserManagement.API.Models;

namespace UserManagement.API.Service
{
    public class NewsletterService : INewsletterService
    {
        private readonly UserManagementContext _userManagementContext;

        public NewsletterService(UserManagementContext userManagementContext)
        {
            _userManagementContext = userManagementContext;
        }

        public void SignUpNewsletter(string email)
        {
            _userManagementContext.NewsletterSubsriptions.Add(new NewsletterSubscription(email));

            _userManagementContext.SaveChanges();
        }
    }
}

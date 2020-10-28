using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iBookStoreCommon.Infrastructure;

namespace UserManagement.API.Models
{
    public class NewsletterSubscription
    {
        public NewsletterSubscription(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new HttpResponseException("Email is invalid.");

            Email = email;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }

        public int Id { get; set; }

        public string Email { get; set; }
    }
}

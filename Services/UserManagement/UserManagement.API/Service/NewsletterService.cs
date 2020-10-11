using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace UserManagement.API.Service
{
    public class NewsletterService : INewsletterService
    {
        private readonly ConcurrentBag<string> _emailBag = new ConcurrentBag<string>();

        public void AddEmailToBag(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException(nameof(email));

            _emailBag.Add(email);
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
    }
}

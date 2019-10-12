using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Identity.API.Data
{
    public static class ApplicationDbInitializer
    {
        private static readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public static void Initialize(ApplicationDbContext context) {
            if (!context.Users.Any()) {
                context.Users.AddRange(GetPreconfiguredUsers());
            }

            context.SaveChanges();
        }

        private static IEnumerable<ApplicationUser> GetPreconfiguredUsers() {
            var user =
            new ApplicationUser() {
                CardHolderName = "DemoUser",
                CardNumber = "4012401240124012",
                CardType = 1,
                City = "Auckland",
                Country = "New Zealand",
                Email = "ibookStore.demo@gmail.com",
                Expiration = "12/21",
                Id = Guid.NewGuid().ToString(),
                LastName = "DemoLastName",
                FirstName = "DemoUser",
                PhoneNumber = "1234567890",
                UserName = "ibookStore.demo@gmail.com",
                ZipCode = "1010",
                State = "AUCK",
                Street = "111 Queen St",
                NormalizedEmail = "IBOOKSTORE.DEMO@GMAIL.COM",
                NormalizedUserName = "IBOOKSTORE.DEMO@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "123123");

            return new List<ApplicationUser>()
            {
                user
            };
        }
    }
}
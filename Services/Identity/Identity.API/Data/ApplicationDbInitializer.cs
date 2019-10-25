using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API.Data
{
    public static class ApplicationDbInitializer
    {
        private static readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public static async Task Initialize(ApplicationDbContext context, IServiceProvider serviceProvider, IConfiguration Configuration) {
            //adding customs roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames) {
                // creating the roles and seeding them to the database
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (!context.Users.Any()) {
                var customers = GetPreconfiguredCustomers().ToList();
                context.Users.AddRange(customers);

                var admins = GetPreconfiguredAdmins();
                context.Users.AddRange(admins);

                context.SaveChanges();

                foreach (var customer in customers) {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }

                foreach (var admin in admins) {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

        }

        private static IEnumerable<ApplicationUser> GetPreconfiguredCustomers() {
            var user = new ApplicationUser() {
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

        private static IEnumerable<ApplicationUser> GetPreconfiguredAdmins() {
            var user = new ApplicationUser() {
                CardHolderName = "DemoAdmin",
                CardNumber = "4011401140114011",
                CardType = 2,
                City = "Auckland",
                Country = "New Zealand",
                Email = "ibookStore.admin@gmail.com",
                Expiration = "12/22",
                Id = Guid.NewGuid().ToString(),
                LastName = "AdminLastName",
                FirstName = "AdminUser",
                PhoneNumber = "0987654321",
                UserName = "ibookStore.admin@gmail.com",
                ZipCode = "1020",
                State = "AUCK",
                Street = "222 Hobson St",
                NormalizedEmail = "IBOOKSTORE.ADMIN@GMAIL.COM",
                NormalizedUserName = "IBOOKSTORE.ADMIN@GMAIL.COM",
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
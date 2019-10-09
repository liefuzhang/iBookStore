using System.Collections.Generic;
using System.Linq;
using Membership.API.Controllers;
using Membership.API.Models;

namespace Membership.API.Infrastructure
{
    public static class DbInitializer
    {
        public static void Initialize(MembershipContext context)
        {
            if (!context.MembershipTypes.Any())
            {
                context.MembershipTypes.AddRange(GetPreconfiguredMembershipTypes());
            }

            context.SaveChanges();
        }

        private static IEnumerable<MembershipType> GetPreconfiguredMembershipTypes()
        {
            return new List<MembershipType>()
            {
                new MembershipType { Name = "Bronze", Description = "Basic access" },
                new MembershipType { Name = "Silver", Description = "Basic access + access to all group classes" },
                new MembershipType { Name = "Gold", Description = "Basic access + access to all group classes + ability to bring a gym buddy" }
            };
        }
    }
}
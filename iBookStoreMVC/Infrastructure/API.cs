using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.Infrastructure
{
    public static class API
    {
        public static class Membership
        {
            public static string GetMembershipTypes(string baseUrl)
            {
                return $"{baseUrl}membershipTypes";
            }
        }
    }
}

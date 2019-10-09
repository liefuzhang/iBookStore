using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.Infrastructure
{
    public static class API
    {
        public static class Catalog
        {
            public static string GetCatalogTypes(string baseUrl)
            {
                return $"{baseUrl}catalogItems";
            }
        }
    }
}

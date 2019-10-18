using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.Infrastructure
{
    public static class API
    {
        public static class Basket
        {
            public static string AddItemToBasket(string baseUrl) => $"{baseUrl}items";

            public static string GetBasket(string baseUrl, string basketId) => $"{baseUrl}/{basketId}";

            public static string UpdateBasketItem(string baseUrl) => $"{baseUrl}items";

            public static string GetOrderDraft(string baseUrl, string basketId) => $"{baseUrl}/orderDraft/{basketId}";
        }

        public static class Catalog
        {
            public static string GetCatalogItems(string baseUrl)
            {
                return $"{baseUrl}catalogItems";
            }
        }
    }
}

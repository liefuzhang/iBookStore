﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.Infrastructure
{
    public static class API
    {
        public static class Basket
        {
            public static string AddItemToBasket(string baseUrl) => $"{baseUrl}/items";

            public static string GetBasket(string baseUrl, string basketId) => $"{baseUrl}/{basketId}";

            public static string UpdateBasketItem(string baseUrl) => $"{baseUrl}/items";

            public static string GetOrderDraft(string baseUrl, string basketId) => $"{baseUrl}/orderDraft/{basketId}";
        }

        public static class Catalog
        {
            public static string GetCatalogItems(string baseUrl, int page, int take, int? categoryId)
            {
                var filter = "";
                if (categoryId.HasValue)
                {
                    filter = $"/category/{categoryId.Value}";
                }

                return $"{baseUrl}/catalogItems{filter}?pageIndex={page}&pageSize={take}";
            }

            public static string GetCatalogItem(string baseUrl, int catalogItemId) {
                return $"{baseUrl}/items/{catalogItemId}";
            }

            public static string GetCategories(string baseUrl)
            {
                return $"{baseUrl}/categories";
            }
        }

        public static class Order
        {
            public static string PlaceOrder(string baseUrl) {
                return $"{baseUrl}/placeOrder";
            }

            public static string GetAllMyOrders(string baseUrl) {
                return baseUrl;
            }

            public static string GetAllOrders(string baseUrl) {
                return $"{baseUrl}/allOrders";
            }

            public static string CancelOrder(string baseUrl) {
                return $"{baseUrl}/cancelOrder";
            }

            public static string GetOrder(string baseUrl, string id) {
                return $"{baseUrl}/{id}";
            }

            public static string ShipOrder(string baseUrl)
            {
                return $"{baseUrl}/shipOrder";
            }
        }
    }
}
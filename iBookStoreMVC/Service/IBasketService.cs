﻿using System.Collections.Generic;
using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IBasketService
    {
        Task AddItemToBasket(ApplicationUser user, int productId);
        Task<Basket> GetBasket(ApplicationUser user);
        Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);
        Task<Order> GetOrderDraft(string basketId);
        Task ClearBasket(string basketId);
    }
}
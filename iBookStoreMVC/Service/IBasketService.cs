﻿using System.Threading.Tasks;
using iBookStoreMVC.ViewModels;

namespace iBookStoreMVC.Service
{
    public interface IBasketService
    {
        Task AddItemToBasket(ApplicationUser user, int productId);
        Task<Basket> GetBasket(ApplicationUser user);
    }
}
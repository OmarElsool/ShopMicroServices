﻿namespace Basket.API.Models;

public class ShoppingCart
{
    public string UserName { get; set; } = default!;
    public List<ShoppingCartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(x=>x.Quantity * x.Price);
    public ShoppingCart(string userName)
    {
        UserName = userName;
    }
    //required for mapping
    public ShoppingCart()
    {
    }
}

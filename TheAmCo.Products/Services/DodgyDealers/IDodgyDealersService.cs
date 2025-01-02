using System;

namespace TheAmCo.Products.Services.DodgeyDealers;

public interface IDodgyDealersService
{
    Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync();
}
using System;

namespace TheAmCo.Products.Services.DodgeyDealers;

public interface IDodgyDealersService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
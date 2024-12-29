using System;

namespace TheAmCo.Products.Services.UnderCutters;

public interface IProductsRepo
{
    Task<IEnumerable<Product>> GetProductsAsync();
}
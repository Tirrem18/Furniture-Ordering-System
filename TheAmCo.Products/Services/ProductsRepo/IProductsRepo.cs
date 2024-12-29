using System;

namespace TheAmCo.Products.Services.ProductsRepo;

public interface IProductsRepo
{
    Task<IEnumerable<Product>> GetProductsAsync();
}
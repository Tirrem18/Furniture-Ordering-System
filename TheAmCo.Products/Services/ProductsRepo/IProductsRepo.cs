using System;

namespace ThAmCo.Products.Services.ProductsRepo;

public interface IProductsRepo
{
    Task<IEnumerable<Product>> GetProductsAsync();
}
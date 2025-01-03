using System;


namespace ThAmCo.Products.Services.ProductsRepo;

public interface IProductsRepo
{
    Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync();
}
using System;
using TheAmCo.Products.Data.Products;


namespace ThAmCo.Products.Services.ProductsRepo;

public interface IProductsRepo
{
    Task<IEnumerable<Product>> GetLocalProductsAsync();
    Task<IEnumerable<Product>> GetUnderCuttersProductsAsync();
    Task<IEnumerable<Product>> GetDodgyDealersProductsAsync();
}
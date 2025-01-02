using System;

namespace ThAmCo.Products.Services.ProductsRepo;

public class ProductRepoFake : IProductsRepo
{
    private readonly Product[] _products=
    {
        new Product { Id = 1, Name = "FAKEREPO Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X" },
        new Product { Id = 2, Name = "FAKEREPO Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y" }
    };
    public Task<IEnumerable<Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
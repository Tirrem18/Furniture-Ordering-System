using System;

namespace TheAmCo.Products.Services.UnderCutters;

public class UnderCuttersServiceFake : IUnderCuttersService
{
    private readonly TheAmCo.Products.Data.Products.Product[] _products=
    {
        new TheAmCo.Products.Data.Products.Product { Id = 4, Name = "UNDERCUTTERS Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X" },
        new TheAmCo.Products.Data.Products.Product { Id = 5, Name = "UNDERCUTTERS Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y" }
    };
    
    public Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
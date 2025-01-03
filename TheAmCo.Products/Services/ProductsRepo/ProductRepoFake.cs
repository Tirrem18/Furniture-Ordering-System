using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheAmCo.Products.Data.Products;

namespace ThAmCo.Products.Services.ProductsRepo;

public class ProductRepoFake : IProductsRepo
{
    private readonly TheAmCo.Products.Data.Products.Product[] _products =
    {
        new TheAmCo.Products.Data.Products.Product { Id = 1, Name = "FAKEREPO Placeholder 1", Description = "Sample description 1", Price = 10.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 1, BrandName = "Brand X", Source = "FakeRepo" },
        new TheAmCo.Products.Data.Products.Product { Id = 2, Name = "FAKEREPO Placeholder 2", Description = "Sample description 2", Price = 15.99m, InStock = false, ExpectedRestock = DateTime.UtcNow.AddDays(5), CategoryId = 2, CategoryName = "Category B", BrandId = 2, BrandName = "Brand Y", Source = "FakeRepo" }
    };

    public Task<IEnumerable<Product>> GetLocalProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }

    public Task<IEnumerable<Product>> GetUnderCuttersProductsAsync()
    {
        var underCuttersProducts = new[]
        {
            new Product { Id = 3, Name = "UNDERCUTTERS Placeholder 1", Description = "Sample description 1", Price = 10.49m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 3, BrandName = "Brand Z", Source = "UnderCutters" }
        };

        return Task.FromResult(underCuttersProducts.AsEnumerable());
    }

    public Task<IEnumerable<Product>> GetDodgyDealersProductsAsync()
    {
        var dodgyDealersProducts = new[]
        {
            new Product { Id = 4, Name = "DODGYDEALERS Placeholder 1", Description = "Sample description 1", Price = 9.99m, InStock = true, CategoryId = 1, CategoryName = "Category A", BrandId = 4, BrandName = "Brand Y", Source = "DodgyDealers" }
        };

        return Task.FromResult(dodgyDealersProducts.AsEnumerable());
    }

public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var localProducts = await GetLocalProductsAsync();
        var underCuttersProducts = await GetUnderCuttersProductsAsync();
        var dodgyDealersProducts = await GetDodgyDealersProductsAsync();

        var allProducts = localProducts
            .Concat(underCuttersProducts)
            .Concat(dodgyDealersProducts)
            .ToList();

        return allProducts;
    }

}

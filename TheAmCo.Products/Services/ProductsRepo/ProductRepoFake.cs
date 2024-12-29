using System;

namespace TheAmCo.Products.Services.ProductsRepo;

public class ProductRepoFake : IProductsRepo
{
    private readonly Product[] _products=
    {
        new Product {ID = 1, Name = "Repo Placehodler 1"},
        new Product {ID = 2, Name = "Repo Placehodler 2"},
        new Product {ID = 3, Name = "Repo Placehodler 3"}
    };
    public Task<IEnumerable<Product>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
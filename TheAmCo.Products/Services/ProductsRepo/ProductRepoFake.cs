using System;

namespace TheAmCo.Products.Services.UnderCutters;

public class ProductRepoFake : IUnderCuttersService
{
    private readonly ProductDto[] _products=
    {
        new ProductDto {ID = 1, Name = "Repo Placehodler 1"},
        new ProductDto {ID = 2, Name = "Repo Placehodler 2"},
        new ProductDto {ID = 3, Name = "Repo Placehodler 3"}
    };
    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
using System;

namespace TheAmCo.Products.Services.UnderCutters;

public class UnderCuttersServiceFake : IUnderCuttersService
{
    private readonly ProductDto[] _products=
    {
        new ProductDto {ID = 1, Name = "Product Placehodler 1"},
        new ProductDto {ID = 2, Name = "Product Placehodler 2"},
        new ProductDto {ID = 3, Name = "Product Placehodler 3"}
    };
    public Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var products = _products.AsEnumerable();
        return Task.FromResult(products);
    }
}
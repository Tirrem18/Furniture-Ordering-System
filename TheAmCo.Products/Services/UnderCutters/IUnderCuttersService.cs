using System;

namespace TheAmCo.Products.Services.UnderCutters;

public interface IUnderCuttersService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}
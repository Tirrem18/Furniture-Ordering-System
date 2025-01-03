using System;

namespace TheAmCo.Products.Services.UnderCutters;

public interface IUnderCuttersService
{
    Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync();
}
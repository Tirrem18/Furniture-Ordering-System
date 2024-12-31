using System;
using Microsoft.EntityFrameworkCore;
using TheAmCo.Products.Data.Products;


namespace ThAmCo.Products.Services.ProductsRepo
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly ProductsContext _productsContext;

        public ProductsRepo(ProductsContext productsContext)
        {
            _productsContext = productsContext;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
           var products = await _productsContext.Products.Select(p => new Product
    {
        ID = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        InStock = p.InStock,
        ExpectedRestock = p.ExpectedRestock,
        CategoryId = p.CategoryId,
        CategoryName = p.CategoryName,
        BrandId = p.BrandId,
        BrandName = p.BrandName
    }).ToListAsync();

    return products;
}
    }
}
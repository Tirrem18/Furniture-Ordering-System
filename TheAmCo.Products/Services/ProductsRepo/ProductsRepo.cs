using System;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Data.Products;
using TheAmCo.Products.Services.ProductsRepo;

namespace ThAmCo.Products.Services.ProductsRepo
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly ProductsContext _productsContext;

        ProductsRepo(ProductsContext productsContext)
        {
            _productsContext = productsContext;
        }

        public async Task<IEnumerable<TheAmCo.Products.Services.ProductsRepo.Product>> GetProductsAsync()
        {
            var products = await _productsContext.Products.Select(p => new TheAmCo.Products.Services.ProductsRepo.Product
            {
                ID = p.Id,
                Name = p.Name
            }).ToListAsync();

            return products;
        }
    }
}
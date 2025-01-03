using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheAmCo.Products.Data.Products;
using TheAmCo.Products.Services.UnderCutters;
using TheAmCo.Products.Services.DodgeyDealers;

namespace ThAmCo.Products.Services.ProductsRepo
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly ProductsContext _productsContext;
        private readonly IUnderCuttersService _underCuttersService;
        private readonly IDodgyDealersService _dodgyDealersService;

        public ProductsRepo(ProductsContext productsContext, IUnderCuttersService underCuttersService, IDodgyDealersService dodgyDealersService)
        {
            _productsContext = productsContext;
            _underCuttersService = underCuttersService;
            _dodgyDealersService = dodgyDealersService;
        }

        public async Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
        {
            // Fetch local products from SQL database
            var localProducts = await _productsContext.Products.ToListAsync();

            // Fetch external products
            var externalProducts = await FetchExternalProductsAsync();

            // Combine and deduplicate products
            var mergedProducts = MergeAndProcessProducts(localProducts, externalProducts);

            // Save merged products to the database
            await SaveToDatabaseAsync(mergedProducts);

            return mergedProducts;
        }

        private async Task<List<TheAmCo.Products.Data.Products.Product>> FetchExternalProductsAsync()
        {
            var underCuttersProducts = await _underCuttersService.GetProductsAsync();
            var dodgyDealersProducts = await _dodgyDealersService.GetProductsAsync();

            // Map external DTOs to the database Product type
            return underCuttersProducts.Concat(dodgyDealersProducts)
                .Select(dto => new TheAmCo.Products.Data.Products.Product
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    InStock = dto.InStock,
                    ExpectedRestock = dto.ExpectedRestock,
                    CategoryId = dto.CategoryId,
                    CategoryName = dto.CategoryName,
                    BrandId = dto.BrandId,
                    BrandName = dto.BrandName,
                    Source = dto.Source
                })
                .ToList();
        }

        private List<TheAmCo.Products.Data.Products.Product> MergeAndProcessProducts(
            List<TheAmCo.Products.Data.Products.Product> localProducts,
            List<TheAmCo.Products.Data.Products.Product> externalProducts)
        {
            var allProducts = localProducts.Concat(externalProducts).ToList();

            return allProducts.GroupBy(p => p.Name).Select(group =>
            {
                // Find the cheapest product
                var cheapestProduct = group.OrderBy(p =>
                    p.Source == "Local" ? p.Price : p.Price * 0.89m).First();

                // Apply 10% markup if the product is from an external source
                if (cheapestProduct.Source != "Local")
                {
                    cheapestProduct.Price = Math.Round(cheapestProduct.Price * 1.10m, 2);
                }

                return cheapestProduct;
            }).ToList();
        }

        private async Task SaveToDatabaseAsync(IEnumerable<TheAmCo.Products.Data.Products.Product> products)
        {
            _productsContext.Products.RemoveRange(_productsContext.Products);
            _productsContext.Products.AddRange((TheAmCo.Products.Data.Products.Product)products);
            await _productsContext.SaveChangesAsync();
        }
    }
}

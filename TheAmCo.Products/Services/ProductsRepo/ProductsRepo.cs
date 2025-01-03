using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheAmCo.Products.Data.Products;
using TheAmCo.Products.Services.UnderCutters;
using TheAmCo.Products.Services.DodgeyDealers;

namespace ThAmCo.Products.Services.ProductsRepo;

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
        // Fetch products from the database
        var localProducts = await _productsContext.Products.ToListAsync();

        // Fetch products from external sources
        var externalProducts = await FetchExternalProductsAsync();

        // Combine, deduplicate, and process products
        var finalProducts = CombineAndProcessProducts(localProducts, externalProducts);

        // Save the final products to the database
        //await SaveProductsToDatabaseAsync(finalProducts);

        return finalProducts;
    }

    private async Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> FetchExternalProductsAsync()
    {
        var underCuttersProducts = await _underCuttersService.GetProductsAsync();
        var dodgyDealersProducts = await _dodgyDealersService.GetProductsAsync();

        // Map external products to database-compatible products
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
            });
    }

    private IEnumerable<TheAmCo.Products.Data.Products.Product> CombineAndProcessProducts(
        IEnumerable<TheAmCo.Products.Data.Products.Product> localProducts,
        IEnumerable<TheAmCo.Products.Data.Products.Product> externalProducts)
    {
        return localProducts.Concat(externalProducts)
            .GroupBy(p => p.Id)
            .Select(group =>
            {
                // Find the cheapest product, with external products adjusted by 11% for comparison
                var cheapestProduct = group.OrderBy(p => 
                    p.Source == "Local" ? p.Price : p.Price * 0.89m).First();

                // Add 10% markup if the product is from an external source
                if (cheapestProduct.Source != "Local")
                {
                    cheapestProduct.Price = Math.Round(cheapestProduct.Price * 1.10m, 2);
                }

                return cheapestProduct;
            })
            .ToList();
    }

    private async Task SaveProductsToDatabaseAsync(IEnumerable<TheAmCo.Products.Data.Products.Product> products)
    {
        // Clear the current database
        _productsContext.Products.RemoveRange(_productsContext.Products);
        
        // Save the new products
        _productsContext.Products.AddRange(products);
        await _productsContext.SaveChangesAsync();
    }
}

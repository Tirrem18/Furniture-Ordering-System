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

    // Fetch external products and map to the database Product type
    var underCuttersProducts = (await _underCuttersService.GetProductsAsync())
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
            BrandName = dto.BrandName
        })
        .ToList();
        Console.WriteLine($"UnderCutters Products Count: {underCuttersProducts.Count}");

    var dodgyDealersProducts = (await _dodgyDealersService.GetProductsAsync())
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
            BrandName = dto.BrandName
        })
        .ToList();

    // Combine local and external products
    var allProducts = localProducts
        .Concat(underCuttersProducts)
        .Concat(dodgyDealersProducts)
        .ToList();
        Console.WriteLine("Final Products:");
        foreach (var product in localProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, Source: {(product.Id < 4 ? "Local" : "External")}");
        }

    // Deduplicate and apply pricing logic
    var distinctProducts = allProducts
        .GroupBy(p => p.Name) // Group by product name
        .Select(g =>
        {
            // Find the cheapest product using 11% reduction for external products
            var cheapestProduct = g.OrderBy(p =>
            {
                // Reduce external product prices by 11% for comparison
                return localProducts.Any(lp => lp.Name == p.Name) ? p.Price : p.Price * 0.89m;
            }).First();

            // Apply 10% markup if the product is external or replaced
            if (!localProducts.Any(lp => lp.Name == cheapestProduct.Name) || cheapestProduct.Price < g.First().Price)
            {
                cheapestProduct.Price = Math.Round(cheapestProduct.Price * 1.10m, 2); // Add 10% markup
            }

            return cheapestProduct;
        })
        .ToList();

    return (IEnumerable<TheAmCo.Products.Data.Products.Product>)distinctProducts;
}





    }
}

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

    public async Task<IEnumerable<Product>> GetLocalProductsAsync()
    {
        return await _productsContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetUnderCuttersProductsAsync()
    {
        var underCuttersProducts = await _underCuttersService.GetProductsAsync();
        return underCuttersProducts.Select(dto => MapToProduct(dto));
    }

    public async Task<IEnumerable<Product>> GetDodgyDealersProductsAsync()
    {
        var dodgyDealersProducts = await _dodgyDealersService.GetProductsAsync();
        return dodgyDealersProducts.Select(dto => MapToProduct(dto));
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var localProducts = await GetLocalProductsAsync();
        var underCuttersProducts = await GetUnderCuttersProductsAsync();
        var dodgyDealersProducts = await GetDodgyDealersProductsAsync();

        var allProducts = localProducts
            .Concat(underCuttersProducts)
            .Concat(dodgyDealersProducts)
            .ToList();

        return CombineAndProcessProducts(allProducts);
    }

    private Product MapToProduct(Product dto)
    {
        return new Product
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
        };
    }

    private IEnumerable<Product> CombineAndProcessProducts(IEnumerable<Product> allProducts)
    {
        return allProducts
            .GroupBy(p => p.Id)
            .Select(group =>
            {
                //if (group.Select(p => p.Name).Distinct().Count() > 1)
               // {
                    //throw new InvalidOperationException($"Name mismatch detected for Product ID {group.Key}");
                //}

                var cheapestProduct = group.OrderBy(p =>
                    p.Source == "Local" ? p.Price : p.Price * 0.89m).First();

                if (cheapestProduct.Source != "Local")
                {
                    cheapestProduct.Price = Math.Round(cheapestProduct.Price * 1.10m, 2);
                }

                return cheapestProduct;
            })
            .ToList();
    }
}

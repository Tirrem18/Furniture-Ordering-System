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
        var products = await _underCuttersService.GetProductsAsync();
        return products.Select(dto => new Product
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
            Source = "UnderCutters"
        });
    }

    public async Task<IEnumerable<Product>> GetDodgyDealersProductsAsync()
    {
        var products = await _dodgyDealersService.GetProductsAsync();
        return products.Select(dto => new Product
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
            Source = "DodgyDealers"
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
}
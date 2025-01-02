using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Data.Products;
using TheAmCo.Products.Services.DodgeyDealers;
using TheAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers
{
    [ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductsContext _productsContext;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly IDodgyDealersService _dodgyDealersService;

    public ProductsController(ProductsContext productsContext, IUnderCuttersService underCuttersService, IDodgyDealersService dodgyDealersService)
    {
        _productsContext = productsContext;
        _underCuttersService = underCuttersService;
        _dodgyDealersService = dodgyDealersService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            // Fetch local products
            var localProducts = await _productsContext.Products.ToListAsync();

            // Fetch products from UnderCutters
            var underCuttersProducts = await _underCuttersService.GetProductsAsync();

            // Fetch products from DodgyDealers
            var dodgyDealersProducts = await _dodgyDealersService.GetProductsAsync();

            // Combine all products into a single list
            var allProducts = localProducts
                .Select(lp => new
                {
                    Source = "Local",
                    lp.Id,
                    lp.Name,
                    lp.Description,
                    lp.Price,
                    lp.InStock,
                    lp.ExpectedRestock,
                    lp.CategoryId,
                    lp.CategoryName,
                    lp.BrandId,
                    lp.BrandName
                })
                .Concat(underCuttersProducts.Select(up => new
                {
                    Source = "UnderCutters",
                    up.Id,
                    up.Name,
                    up.Description,
                    up.Price,
                    up.InStock,
                    up.ExpectedRestock,
                    up.CategoryId,
                    up.CategoryName,
                    up.BrandId,
                    up.BrandName
                }))
                .Concat(dodgyDealersProducts.Select(dp => new
                {
                    Source = "DodgyDealers",
                    dp.Id,
                    dp.Name,
                    dp.Description,
                    dp.Price,
                    dp.InStock,
                    dp.ExpectedRestock,
                    dp.CategoryId,
                    dp.CategoryName,
                    dp.BrandId,
                    dp.BrandName
                }))
                .ToList();

            return Ok(allProducts); // Return the combined list as JSON
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    [HttpGet("products")]
public async Task<IEnumerable<TheAmCo.Products.Data.Products.Product>> GetProductsAsync()
{
    // Fetch local products from SQL database
    var localProducts = await _productsContext.Products.ToListAsync();

    // Fetch external products
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
            BrandName = dto.BrandName,
            Source = "UnderCutters"
        })
        .ToList();

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
            BrandName = dto.BrandName,
            Source = "DodgyDealers"
        })
        .ToList();

    // Combine external products into a single list for processing
    var externalProducts = underCuttersProducts.Concat(dodgyDealersProducts).ToList();

    // Process external products
    foreach (var externalProduct in externalProducts)
    {
        // Find matching product in local repository
        var matchingLocalProduct = localProducts.FirstOrDefault(lp => lp.Id == externalProduct.Id);

        if (matchingLocalProduct != null)
        {
            // Calculate external price with 11% reduction
            var externalPriceWithReduction = externalProduct.Price * 0.89m;

            // Update local product if external price is cheaper
            if (externalPriceWithReduction < matchingLocalProduct.Price)
            {
                matchingLocalProduct.Price = Math.Round(externalProduct.Price * 1.10m, 2); // Add 10% markup
                matchingLocalProduct.Source = externalProduct.Source;
                matchingLocalProduct.InStock = externalProduct.InStock;
                matchingLocalProduct.ExpectedRestock = externalProduct.ExpectedRestock;
                matchingLocalProduct.BrandId = externalProduct.BrandId;
                matchingLocalProduct.BrandName = externalProduct.BrandName;
            }
        }
        else
        {
            
            localProducts.Add(new TheAmCo.Products.Data.Products.Product
            {
                Id = externalProduct.Id,
                Name = externalProduct.Name,
                Description = externalProduct.Description,
                Price = Math.Round(externalProduct.Price * 1.10m, 2),
                InStock = externalProduct.InStock,
                ExpectedRestock = externalProduct.ExpectedRestock,
                CategoryId = externalProduct.CategoryId,
                CategoryName = externalProduct.CategoryName,
                BrandId = externalProduct.BrandId,
                BrandName = externalProduct.BrandName,
                Source = externalProduct.Source
            });
        }
    }

    return localProducts;
}

    
}

}

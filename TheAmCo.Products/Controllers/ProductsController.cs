using Microsoft.AspNetCore.Mvc;
using TheAmCo.Products.Data.Products;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Services.UnderCutters;
using TheAmCo.Products.Services.DodgeyDealers;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsRepo _productsRepo;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly IDodgyDealersService _dodgyDealersService;

    public ProductsController(IProductsRepo productsRepo, IUnderCuttersService underCuttersService, IDodgyDealersService dodgyDealersService)
    {
        _productsRepo = productsRepo;
        _underCuttersService = underCuttersService;
        _dodgyDealersService = dodgyDealersService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            // Fetch products from ProductRepoFake
            var repoProducts = await _productsRepo.GetProductsAsync();

            // Fetch products from UnderCuttersServiceFake
            var underCuttersProducts = await _underCuttersService.GetProductsAsync();

            // Fetch products from DodgyDealersServiceFake
            var dodgyDealersProducts = await _dodgyDealersService.GetProductsAsync();

            // Combine all products
            var allProducts = repoProducts
                .Concat(underCuttersProducts)
                .Concat(dodgyDealersProducts)
                .ToList();

            return Ok(allProducts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}

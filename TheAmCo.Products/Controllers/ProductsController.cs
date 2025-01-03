using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Services.ProductsRepo;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsRepo _productsRepo;

    public ProductsController(IProductsRepo productsRepo)
    {
        _productsRepo = productsRepo;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productsRepo.GetProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}

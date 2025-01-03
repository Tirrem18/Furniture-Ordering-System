using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Services.ProductsRepo;
using System.Threading.Tasks;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("api")]
public class ProductsController : ControllerBase
{
    private readonly IProductsRepo _productsRepo;

    public ProductsController(IProductsRepo productsRepo)
    {
        _productsRepo = productsRepo;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            var allProducts = await _productsRepo.GetProductsAsync();
            return Ok(allProducts);
        }
        catch (InvalidOperationException ex)
        {
            // Handle the specific name mismatch error gracefully
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            // General error handling
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}

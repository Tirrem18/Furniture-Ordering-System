using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly IProductsRepo _productsRepo;

    public DebugController(ILogger<DebugController> logger,
                           IUnderCuttersService underCuttersService,
                           IProductsRepo productRepo)
    {
        _logger = logger;
        _underCuttersService = underCuttersService;
        _productsRepo = productRepo;
    }

    
    [HttpGet("UnderCutters")]
    public async Task<IActionResult> UnderCutters()
    {
        IEnumerable<ProductDto> products = null!;
        try
        {
            products = await _underCuttersService.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using UnderCutters service.");
            products = Array.Empty<ProductDto>();
        }
        return Ok(products.ToList());
    }

      [HttpGet("repo")]
    public async Task<IActionResult> Repo()
    {
        IEnumerable<Product> products = null!;
        try
        {
            products = await _productsRepo.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using repo service.");
            products = Array.Empty<Product>();
        }
        return Ok(products.ToList());
    }

}
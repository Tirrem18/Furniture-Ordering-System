using Microsoft.AspNetCore.Mvc;
using TheAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IUnderCuttersService _underCuttersService;

    public DebugController(ILogger<DebugController> logger,
                           IUnderCuttersService reviewsService)
    {
        _logger = logger;
        _underCuttersService = reviewsService;
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
}
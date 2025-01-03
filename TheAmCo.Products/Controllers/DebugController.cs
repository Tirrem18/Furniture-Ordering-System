using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Services.DodgeyDealers;
using TheAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IUnderCuttersService _underCuttersService;
    private readonly IProductsRepo _productsRepo;
    private readonly IDodgyDealersService _dodgeyDealersService;

    public DebugController(ILogger<DebugController> logger,
                           IUnderCuttersService underCuttersService,
                           IProductsRepo productRepo,
                           IDodgyDealersService dodgeyDealersService) // Added parameter
    {
        _logger = logger;
        _underCuttersService = underCuttersService;
        _productsRepo = productRepo;
        _dodgeyDealersService = dodgeyDealersService; // Assigned value
    }

    [HttpGet("UnderCutters")]
    public async Task<IActionResult> UnderCutters()
    {
        IEnumerable<TheAmCo.Products.Data.Products.Product> products = null!;
        try
        {
            products = await _underCuttersService.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using UnderCutters service.");
            products = Array.Empty<TheAmCo.Products.Data.Products.Product>();
        }
        return Ok(products.ToList());
    }

    [HttpGet("DodgeyDealers")]
    public async Task<IActionResult> DodgeyDealers()
    {
        IEnumerable<TheAmCo.Products.Data.Products.Product> products = null!;
        try
        {
            products = await _dodgeyDealersService.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using DodgyDealers service.");
            products = Array.Empty<TheAmCo.Products.Data.Products.Product>();
        }
        return Ok(products.ToList());
    }

    [HttpGet("repo")]
    public async Task<IActionResult> Repo()
    {
        IEnumerable<TheAmCo.Products.Data.Products.Product> products = null!;
        try
        {
            products = await _productsRepo.GetProductsAsync();
        }
        catch
        {
            _logger.LogWarning("Exception occurred using repo service.");
            products = Array.Empty<TheAmCo.Products.Data.Products.Product>();
        }
        return Ok(products.ToList());
    }
}

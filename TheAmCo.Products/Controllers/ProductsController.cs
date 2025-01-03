using Microsoft.AspNetCore.Mvc;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Data.Products;
using System.Collections.Generic;
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
    var localProducts = await _productsRepo.GetLocalProductsAsync();
    var underCuttersProducts = await _productsRepo.GetUnderCuttersProductsAsync();
    var dodgyDealersProducts = await _productsRepo.GetDodgyDealersProductsAsync();

    var allProducts = localProducts
        .Concat(underCuttersProducts)
        .Concat(dodgyDealersProducts)
        .ToList();

    return Ok(allProducts);
}
}
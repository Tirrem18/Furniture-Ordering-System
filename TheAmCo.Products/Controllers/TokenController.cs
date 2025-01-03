using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheAmCo.Products.Services.DodgeyDealers;
using TheAmCo.Products.Services.UnderCutters;

namespace ThAmCo.Products.Controllers
{
    [ApiController]
    [Route("api")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly IUnderCuttersService _underCuttersService;
        private readonly IDodgyDealersService _dodgeyDealersService;

        public TokenController(
            ILogger<TokenController> logger,
            IUnderCuttersService underCuttersService,
            IDodgyDealersService dodgeyDealersService)
        {
            _logger = logger;
            _underCuttersService = underCuttersService;
            _dodgeyDealersService = dodgeyDealersService;
        }

        [Authorize] // Protect the UnderCutters endpoint
        [HttpGet("undercutters")]
        public async Task<IActionResult> GetUnderCuttersProducts()
        {
            IEnumerable<TheAmCo.Products.Data.Products.Product> products = null!;
            try
            {
                products = await _underCuttersService.GetProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception occurred while fetching products from UnderCutters");
                products = Array.Empty<TheAmCo.Products.Data.Products.Product>();
            }

            return Ok(products.ToList());
        }

        [Authorize] // Protect the DodgeyDealers endpoint
        [HttpGet("dodgeydealers")]
        public async Task<IActionResult> GetDodgeyDealersProducts()
        {
            IEnumerable<TheAmCo.Products.Data.Products.Product> products = null!;
            try
            {
                products = await _dodgeyDealersService.GetProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception occurred while fetching products from DodgeyDealers.");
                products = Array.Empty<TheAmCo.Products.Data.Products.Product>();
            }

            return Ok(products.ToList());
        }
    }
    
}

using DotNetFoundations.Models;
using DotNetFoundations.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetFoundations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;

        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProd(
            [FromBody] Products product)
        {
            await _productsService.AddAsync(product);

            return Ok(new
            {
                message = "Product added successfully"
            });
        }
    }
}
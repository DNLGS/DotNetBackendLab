using DotNetFoundations.DTOs.Products.Requests;
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
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductRequest product)
        {
            await _productsService.CreateAsync(product);

            return Ok(new
            {
                message = "Product added successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsFilter([FromQuery] QueryProductRequest filter)
        {
            var products = await _productsService.QueryAsync(filter);
            return Ok(new {
                products = products,
            });
        }
    }
}

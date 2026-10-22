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
            await _productsService.AddAsync(product);

            return Ok(new
            {
                message = "Product added successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsFilter([FromQuery] QueryProductRequest filter)
        {
            var entities = await _productsService.GetByFilterAsync(filter);
            var products = entities.Select(ProductsService.ToResponse).ToList();
            return Ok(new {
                products = products,
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromQuery] DeleteProductRequest request)
        {
            var prod = await _productsService.GetByIdAsync(request.ProductsId);
            if (prod == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            await _productsService.DeleteAsync(prod, request.Version);
            return Ok(new
            {
                message = "Product deleted successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            await _productsService.UpdateAsync(request);
            return Ok(new
            {
                message = "Product updated successfully"
            });
        }
    }
}

using DotNetFoundations.DTOs.Orders.Requests;
using DotNetFoundations.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetFoundations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService service)
        {
            _orderService = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest order)
        {
            await _orderService.CreateAsync(order);
            return Ok(new {
                    message = "Order created successfully",
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersFilter([FromQuery] QueryOrderRequest filter)
        {
            var orders = await _orderService.QueryAsync(filter);
            return Ok(new
            {
                orders = orders,
            });
        }
    }
}

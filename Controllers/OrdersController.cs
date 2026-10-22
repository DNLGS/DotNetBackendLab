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
            await _orderService.AddAsync(order);
            return Ok(new {
                    message = "Order created successfully",
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersFilter([FromQuery] QueryOrderRequest filter)
        {
            var entities = await _orderService.GetByFilterAsync(filter);
            var orders = entities.Select(OrderService.ToResponse).ToList();
            return Ok(new
            {
                orders = orders,
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder([FromQuery] DeleteOrderRequest request)
        {
            var order = await _orderService.GetByIdAsync(request.OrdersId);
            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }
            await _orderService.DeleteAsync(order, request.Version);
            return Ok(new
            {
                message = "Order deleted successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
        {
            await _orderService.UpdateAsync(request);
            return Ok(new
            {
                message = "Order updated successfully"
            });
        }
    }
}

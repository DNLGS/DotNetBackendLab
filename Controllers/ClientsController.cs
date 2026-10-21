using DotNetFoundations.DTOs.Clients.Requests;
using DotNetFoundations.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetFoundations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientsService _clientsService;
        public ClientsController(ClientsService service)
        {
            _clientsService = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest client)
        {
            await _clientsService.CreateAsync(client);
            return Ok(new
            {
                message = "Client created successfully",
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetClientsFilter([FromQuery] QueryClientRequest filter)
        {
            var clients = await _clientsService.QueryAsync(filter);
            return Ok(new
            {
                clients = clients,
            });
        }
    }
}

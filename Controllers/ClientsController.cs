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
            await _clientsService.AddAsync(client);
            return Ok(new
            {
                message = "Client created successfully",
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetClientsFilter([FromQuery] QueryClientRequest filter)
        {
            var entities = await _clientsService.GetByFilterAsync(filter);
            var clients = entities.Select(ClientsService.ToResponse).ToList();
            return Ok(new
            {
                clients = clients,
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteClient([FromQuery] DeleteClientRequest request)
        {
            var client = await _clientsService.GetByIdAsync(request.ClientsId);
            if (client == null)
            {
                return NotFound(new
                {
                    message = "Client not found"
                });
            }

            await _clientsService.DeleteAsync(client, request.Version);
            return Ok(new
            {
                message = "Client deleted successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientRequest request)
        {
            await _clientsService.UpdateAsync(request);
            return Ok(new
            {
                message = "Client updated successfully"
            });
        }
    }
}

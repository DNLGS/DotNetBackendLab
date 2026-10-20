using DotNetFoundations.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetFoundations.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthCheckController : ControllerBase
    {
        private readonly DatabaseService _db;

        public HealthCheckController(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("database")]
        public async Task<IActionResult> CheckDatabase(
            CancellationToken cancellationToken)
        {
            bool connected =
                await _db.CheckConnection(
                    cancellationToken);

            if (!connected)
            {
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    database = "disconnected"
                });
            }

            return Ok(new
            {
                status = "healthy",
                database = "connected"
            });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.Models
{
    public class Clients
    {
        public Guid ClientsId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string Phone { get; set; } = string.Empty;
        public ICollection<Orders>? Orders { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

namespace DotNetFoundations.Models;

public class Clients
{
    public Guid ClientsId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Orders> Orders { get; set; } = [];
}

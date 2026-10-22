namespace DotNetFoundations.Models;

public class Products
{
    public Guid ProductsId { get; set; } = Guid.NewGuid();
    public uint Version { get; set; }
    public required string Name { get; set; }
    public decimal SuggestedPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

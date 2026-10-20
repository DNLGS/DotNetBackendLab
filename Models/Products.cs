namespace DotNetFoundations.Models
{
    public class Products
    {
        public Guid ProductsId { get; set; } = Guid.NewGuid();
        public required string Name { get; set; } = string.Empty;
        public required double SuggestedPrice { get; set; } = 0.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

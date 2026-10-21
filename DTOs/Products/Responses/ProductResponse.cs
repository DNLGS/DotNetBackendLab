namespace DotNetFoundations.DTOs.Products.Responses;

public class ProductResponse
{
    public Guid ProductsId { get; init; }
    public required string Name { get; init; }
    public decimal SuggestedPrice { get; init; }
    public DateTime CreatedAt { get; init; }
}

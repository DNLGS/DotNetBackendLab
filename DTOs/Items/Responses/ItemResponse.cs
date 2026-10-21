using DotNetFoundations.DTOs.Products.Responses;

namespace DotNetFoundations.DTOs.Items.Responses;

public class ItemResponse
{
    public Guid ItemsId { get; init; }
    public Guid? OrdersId { get; init; }
    public Guid ProductId { get; init; }
    public ProductResponse? Product { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
}

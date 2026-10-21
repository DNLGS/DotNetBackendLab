using DotNetFoundations.Enums;
using DotNetFoundations.DTOs.Clients.Responses;
using DotNetFoundations.DTOs.Items.Responses;

namespace DotNetFoundations.DTOs.Orders.Responses;

public class OrderResponse
{
    public Guid OrdersId { get; init; }
    public Guid ClientId { get; init; }
    public ClientResponse? Client { get; init; }
    public required List<ItemResponse> Items { get; init; }
    public OrderStatus Status { get; init; }
    public decimal Descount { get; init; }
    public decimal Increase { get; init; }
    public decimal GrossValue { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
}

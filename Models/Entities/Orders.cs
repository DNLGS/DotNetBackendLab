using DotNetFoundations.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetFoundations.Models;

public class Orders
{
    public Guid OrdersId { get; set; } = Guid.NewGuid();
    public required Guid ClientId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Descount { get; set; }
    public decimal Increase { get; set; }
    public decimal GrossValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Clients? Client { get; set; }
    public ICollection<Items> Items { get; set; } = [];

    [NotMapped]
    public decimal Total => GrossValue - Descount + Increase;
}

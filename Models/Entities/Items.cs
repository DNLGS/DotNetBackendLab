using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetFoundations.Models;

public class Items
{
    public Guid ItemsId { get; set; } = Guid.NewGuid();
    public Guid? OrdersId { get; set; }
    public required Guid ProductId { get; set; }
    public required decimal Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Products? Product { get; set; }

    [NotMapped]
    public decimal Total => Quantity * UnitPrice;
}

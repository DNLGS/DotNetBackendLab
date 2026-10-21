using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;
using DotNetFoundations.Enums;
using DotNetFoundations.DTOs.Items.Requests;

namespace DotNetFoundations.DTOs.Orders.Requests;

public class UpdateOrderRequest
{
    [NotEmptyGuid]
    public required Guid OrdersId { get; set; }
    [NotEmptyGuid]
    public required Guid ClientId { get; set; }
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal Descount { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal Increase { get; set; }
    [Required]
    public required List<UpdateItemRequest> Items { get; set; }
}

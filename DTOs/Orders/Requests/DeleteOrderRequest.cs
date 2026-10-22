using DotNetFoundations.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Orders.Requests;

public class DeleteOrderRequest
{
    [NotEmptyGuid]
    public required Guid OrdersId { get; set; }
    [Range(typeof(uint), "1", "4294967295", ParseLimitsInInvariantCulture = true)]
    public required uint Version { get; set; }
}

using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Items.Requests;

public class UpdateItemRequest
{
    [NotEmptyGuid]
    public required Guid ItemsId { get; set; }
    [NotEmptyGuid]
    public required Guid ProductId { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", MinimumIsExclusive = true, ParseLimitsInInvariantCulture = true)]
    public required decimal Quantity { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public required decimal UnitPrice { get; set; }
}

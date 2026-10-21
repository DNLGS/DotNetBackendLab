using DotNetFoundations.DTOs.Common;
using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;
using DotNetFoundations.Enums;

namespace DotNetFoundations.DTOs.Orders.Requests;

public class QueryOrderRequest : QueryRequest
{
    [NotEmptyGuid]
    public Guid? ClientId { get; set; }
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus? Status { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal? Descount { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal? Increase { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal? GrossValue { get; set; }
    public DateTime? CreatedAt { get; set; }
}

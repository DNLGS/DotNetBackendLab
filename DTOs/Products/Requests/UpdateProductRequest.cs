using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;
using DotNetFoundations.Interfaces;

namespace DotNetFoundations.DTOs.Products.Requests;

public class UpdateProductRequest : IVersionedRequest
{
    [NotEmptyGuid]
    public required Guid ProductsId { get; set; }
    [Range(typeof(uint), "1", "4294967295", ParseLimitsInInvariantCulture = true)]
    public required uint Version { get; set; }
    [Required]
    public required string Name { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal SuggestedPrice { get; set; }
}

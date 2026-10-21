using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Products.Requests;

public class UpdateProductRequest
{
    [NotEmptyGuid]
    public required Guid ProductsId { get; set; }
    [Required]
    public required string Name { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal SuggestedPrice { get; set; }
}

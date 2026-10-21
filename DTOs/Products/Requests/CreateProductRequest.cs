using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Products.Requests;

public class CreateProductRequest
{
    [Required]
    public required string Name { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal SuggestedPrice { get; set; }
}

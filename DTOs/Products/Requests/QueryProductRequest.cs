using DotNetFoundations.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Products.Requests;

public class QueryProductRequest : QueryRequest
{
    public string? Name { get; set; }
    public string? NameContains { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
    public decimal? MinSuggestedPrice { get; set; }
}

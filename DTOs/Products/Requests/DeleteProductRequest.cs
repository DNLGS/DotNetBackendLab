using DotNetFoundations.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Products.Requests;

public class DeleteProductRequest
{
    [NotEmptyGuid]
    public required Guid ProductsId { get; set; }
    [Range(typeof(uint), "1", "4294967295", ParseLimitsInInvariantCulture = true)]
    public required uint Version { get; set; }
}

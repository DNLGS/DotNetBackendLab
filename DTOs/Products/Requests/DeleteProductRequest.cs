using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Products.Requests;

public class DeleteProductRequest
{
    [NotEmptyGuid]
    public required Guid ProductsId { get; set; }
}

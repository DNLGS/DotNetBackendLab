using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Items.Requests;

public class DeleteItemRequest
{
    [NotEmptyGuid]
    public required Guid ItemsId { get; set; }
}

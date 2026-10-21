using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class DeleteClientRequest
{
    [NotEmptyGuid]
    public required Guid ClientsId { get; set; }
}

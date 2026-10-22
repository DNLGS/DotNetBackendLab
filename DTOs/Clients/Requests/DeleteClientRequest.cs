using DotNetFoundations.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class DeleteClientRequest
{
    [NotEmptyGuid]
    public required Guid ClientsId { get; set; }
    [Range(typeof(uint), "1", "4294967295", ParseLimitsInInvariantCulture = true)]
    public required uint Version { get; set; }
}

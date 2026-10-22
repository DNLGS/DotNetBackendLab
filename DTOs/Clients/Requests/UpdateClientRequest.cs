using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;
using DotNetFoundations.Interfaces;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class UpdateClientRequest : IVersionedRequest
{
    [NotEmptyGuid]
    public required Guid ClientsId { get; set; }
    [Range(typeof(uint), "1", "4294967295", ParseLimitsInInvariantCulture = true)]
    public required uint Version { get; set; }
    public string? Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
}

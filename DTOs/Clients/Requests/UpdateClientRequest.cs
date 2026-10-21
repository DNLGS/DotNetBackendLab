using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class UpdateClientRequest
{
    [NotEmptyGuid]
    public required Guid ClientsId { get; set; }
    [Required]
    public required string Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
}

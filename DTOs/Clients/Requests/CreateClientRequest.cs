using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class CreateClientRequest
{
    [Required]
    public required string Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
}

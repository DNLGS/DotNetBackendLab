namespace DotNetFoundations.DTOs.Clients.Responses;

public class ClientResponse
{
    public Guid ClientsId { get; init; }
    public uint Version { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public DateTime CreatedAt { get; init; }
}

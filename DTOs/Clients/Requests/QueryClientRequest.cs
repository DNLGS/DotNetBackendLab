using DotNetFoundations.DTOs.Common;

namespace DotNetFoundations.DTOs.Clients.Requests;

public class QueryClientRequest : QueryRequest
{
    public string? Name { get; set; }
    public string? NameContains { get; set; }
    public string? Email { get; set; }
    public string? EmailContains { get; set; }
    public string? Phone { get; set; }
}

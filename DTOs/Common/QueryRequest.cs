namespace DotNetFoundations.DTOs.Common;

public abstract class QueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderBy { get; set; }
    public bool Descending { get; set; }
}

namespace DotNetFoundations.Filters
{
    public abstract class BaseFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}

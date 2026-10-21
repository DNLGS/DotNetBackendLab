namespace DotNetFoundations.Filters
{
    public class ProductFilter : BaseFilter
    {
        public string? Name { get; set; }
        public string? NameContains { get; set; }
        public decimal? MinSuggestedPrice { get; set; }
    }
}

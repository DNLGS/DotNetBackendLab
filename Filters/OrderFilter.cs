using DotNetFoundations.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.Filters
{
    public class OrderFilter : BaseFilter
    {
        public Guid? ClientId { get; set; }
        public OrderStatus? Status { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
        public decimal? Descount { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
        public decimal? Increase { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
        public decimal? GrossValue { get; set; }
        public DateTime? CreatedAt { get; set; }                       
    }
}

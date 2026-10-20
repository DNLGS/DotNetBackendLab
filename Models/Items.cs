using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetFoundations.Models
{
    public class Items
    {
        public Guid ItemsId { get; set; } = Guid.NewGuid();
        public required Guid ProductId { get; set; }
        public required Products Product { get; set; }
        public required double Quantity { get; set; } = 0.0;
        public required double UnitPrice { get; set; } = 0.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public double Total { get { return GetTotal(); } }  
        private double GetTotal()
        {
            return Quantity * UnitPrice;
        }
    }    
}

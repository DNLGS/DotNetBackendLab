using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetFoundations.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Orders
    {
        public Guid OrdersId { get; set; } = Guid.NewGuid();
        public required Guid ClientId { get; set; }
        public required Clients Client { get; set; }
        public required ICollection<Items> Items { get; set; } = new List<Items>();
        public required OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Range(0, double.MaxValue)]
        public double Descount { get; set; } = 0.0;
        [Range(0, double.MaxValue)]
        public double Increase { get; set; } = 0.0;
        [Range(0, double.MaxValue)]
        public required double GrossValue { get; set; } = 0.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public double Total { get { return GetTotal(); } }
        
        private double GetTotal()
        {
            return GrossValue - Descount + Increase;
        }
    }    
}

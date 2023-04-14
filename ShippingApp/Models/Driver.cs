using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class Driver
    {
        [Key]
        public Guid driverId { get; set; }
        public Guid checkpointLocation { get; set; }
        public bool isAvailable { get; set; }
    }
}

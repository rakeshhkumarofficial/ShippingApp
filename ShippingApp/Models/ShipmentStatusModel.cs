using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class ShipmentStatusModel
    {
        [Key]
        public Guid shipmentStatusId { get; set; } 
        public Guid shipmentId { get; set; }
        public string shipmentStatus { get; set; } 
        public Guid currentLocation { get; set; } 
        public DateTime lastUpdated { get; set; }
    }
}

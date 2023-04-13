namespace ShippingApp.Models
{
    public class ShipmentStatusModel
    {
        public Guid shipmentStatusId { get; set; } 
        public Guid shipmentId { get; set; }
        public string shipmentStatus { get; set; } 
        public string currentLocation { get; set; } 
        public DateTime lastUpdated { get; set; }
    }
}

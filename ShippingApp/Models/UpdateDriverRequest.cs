namespace ShippingApp.Models
{
    public class UpdateDriverRequest
    {
        public Guid driverId { get; set; }
        public Guid checkpointLocation { get; set; }
        public string shipmentStatus { get; set; }
        public bool isAvailable { get; set; }
    }
}

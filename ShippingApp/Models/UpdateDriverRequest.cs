namespace ShippingApp.Models
{
    public class UpdateDriverRequest
    {
        public Guid driverId { get; set; }
        public Guid checkpointLocation { get; set; }
        public bool isAvailable { get; set; } = false;
    }
}

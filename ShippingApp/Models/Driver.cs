namespace ShippingApp.Models
{
    public class Driver
    {
        public Guid driverId { get; set; }
        public string location { get; set; } = string.Empty;
        public bool isAvailable { get; set; }
    }
}

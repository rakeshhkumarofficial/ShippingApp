namespace ShippingApp.Models
{
    public class AcceptShipmentRequest
    {
        public Guid mapId { get; set; }
        public Guid driverId { get; set; }
        public bool isAccepted { get ; set; }
    }
}

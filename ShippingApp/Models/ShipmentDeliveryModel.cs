namespace ShippingApp.Models
{
    public class ShipmentDeliveryModel
    {
        public ShipmentModel shipment { get; set; } 
        public List<CheckpointModel> checkpoints { get; set; } 
    }
}

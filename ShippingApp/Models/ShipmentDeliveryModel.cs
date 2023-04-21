namespace ShippingApp.Models
{
    public class ShipmentDeliveryModel
    {
        public SendShipmentModel shipment { get; set; } 
        public List<CheckpointModel> checkpoints { get; set; } 
    }
}

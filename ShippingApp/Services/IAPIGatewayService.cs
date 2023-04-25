using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IAPIGatewayService
    {
        public List<CheckpointModel> GetShipmentRoute(Guid shipmentId);
        public List<CheckpointModel> GetCheckpoints(Guid checkpointId);
    }
}

using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IAPIGatewayService
    {
        public List<CheckpointModel> GetShipmentRoute(Guid shipmentId);
        public List<CheckpointModel> GetCheckpoints(Guid checkpointId,string checkpointName);
        public decimal GetCheckpointsDistance(Guid checkpoint1Id , Guid checkpoint2Id);
    }
}

using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IAPIGatewayService
    {
        public List<CheckpointModel> GetCheckpoints(Guid shipmentId);
    }
}

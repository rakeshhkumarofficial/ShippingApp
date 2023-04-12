using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface ICheckpointService
    {
        public Response AddCheckpoint(AddCheckpointRequest addCheckpoint);
        public Response DeleteCheckpoint(Guid checkpointId);
        public Response GetCheckpoints(Guid checkpointId, string? location);
        public Response UpdateCheckpoint(Checkpoint updateCheckpoint);
    }
}

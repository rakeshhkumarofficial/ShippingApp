using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IDriverService
    {
        public Response AddDriver(Driver driver);
        public Response DeleteDriver(Guid driverId);
        public Response GetDriver(Guid driverId, Guid checkpointLocation, bool isAvailable);
        public Response UpdateDriver(Driver updateDriver);
    }
}

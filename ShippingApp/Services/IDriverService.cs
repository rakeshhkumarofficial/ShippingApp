using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IDriverService
    {
        public Response AddDriver(Driver driver);
        public Response DeleteDriver(Guid driverId);
        public Response GetDriver(Guid driverId, string? location, bool isAvailable);
        public Response UpdateDriver(Driver updateDriver);
    }
}

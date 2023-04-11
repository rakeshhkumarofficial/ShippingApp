using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IDriverService
    {
        public Response AddDriver(Driver driver);
        public Response DeleteDriver(Guid driverid);
    }
}

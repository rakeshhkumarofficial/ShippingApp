using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class DriverService : IDriverService
    {
        private readonly ShippingDbContext _dbContext;
        Response response = new Response();
        public DriverService(ShippingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Response AddDriver(Driver driver)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            if (driver.driverId == Guid.Empty)
            {
                response.Message = "Please Enter Driver Id";
                return response;
            }
            if (driver.location == null || driver.location == "")
            {
                response.Message = "Please Enter the Driver Location";
                return response;
            }
            bool IsUserExists = _dbContext.Drivers.Where(d=>d.driverId == driver.driverId).Any();
            if (!IsUserExists)
            {
                _dbContext.Drivers.Add(driver);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
                response.Data = driver;
                response.Message = "New Driver Created";
                return response;
            }
            response.Message = "Driver already Exits";
            return response;
        }

        public Response DeleteDriver(Guid driverid)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(driverid);
            if (driver == null)
            {
                response.Message = "Driver not Found";
                return response;
            }
            _dbContext.Drivers.Remove(driver);
            _dbContext.SaveChanges();
            response.Data = driver;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Driver is Deleted";
            return response;
        }
    }
}

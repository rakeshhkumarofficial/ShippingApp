using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;
using ShippingApp.RabbitMQ;

namespace ShippingApp.Services
{
    public class DriverService : IDriverService
    {
        private readonly ShippingDbContext _dbContext;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        Response response = new Response();
        public DriverService(ShippingDbContext dbContext,IRabbitMQProducer rabbitMQProducer)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
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
            if (driver.checkpointLocation == Guid.Empty)
            {
                response.Message = "Please Enter the Driver CheckpointLocation";
                return response;
            }
            bool IsUserExists = _dbContext.Drivers.Where(d=>d.driverId == driver.driverId).Any();
            if (!IsUserExists)
            {
                _dbContext.Drivers.Add(driver);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
                response.StatusCode = 200;
                response.Data = driver;
                response.Message = "New Driver Created";
                return response;
            }
            response.Message = "Driver already Exits";
            return response;
        }
        public Response DeleteDriver(Guid driverId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(driverId);
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
        public Response GetDriver(Guid driverId, Guid checkpointLocation, bool isAvailable)
        {
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "Drivers List";
            if (driverId == Guid.Empty && checkpointLocation == Guid.Empty)
            {
                var obj = _dbContext.Drivers;
                response.Data = obj;
                return response;
            }
            var drivers = from driver in _dbContext.Drivers where ((driver.driverId == driverId || driverId == Guid.Empty) && (driver.checkpointLocation == checkpointLocation || driver.checkpointLocation == Guid.Empty) && (driver.isAvailable == isAvailable)) select driver;
            response.Data = drivers;
            return response;
        }

        public Response UpdateDriver(UpdateDriverRequest updateDriver)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(updateDriver.driverId);
            if (driver == null)
            {
                response.Message = "Driver Not Found";
                return response;
            }
            if (updateDriver.checkpointLocation != Guid.Empty) { driver.checkpointLocation = updateDriver.checkpointLocation; }
            
            driver.isAvailable = updateDriver.isAvailable;
            var obj = _dbContext.Shippers.Where(s=>s.driverId == updateDriver.driverId).FirstOrDefault();
            if (obj == null)
            {
                _dbContext.SaveChanges();
                response.Data = driver;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Driver Details Updated";
                return response;
            }
            var shipmentStatus = new ShipmentStatusModel()
            {
                shipmentStatusId = Guid.NewGuid(),
                shipmentId = obj.shipmentId,
                shipmentStatus = "Accepted",
                currentLocation = updateDriver.checkpointLocation,
                lastUpdated = DateTime.Now
            };
            if (updateDriver.shipmentStatus != null || updateDriver.shipmentStatus != "") {
                shipmentStatus.shipmentStatus = updateDriver.shipmentStatus;
            }                           
            _rabbitMQProducer.SendProductMessage(shipmentStatus);
            _dbContext.SaveChanges();
            response.Data = driver;
            response.StatusCode = 200;
            response.IsSuccess = true;  
            response.Message = "Driver Details Updated";
            return response;
        }
    }
}

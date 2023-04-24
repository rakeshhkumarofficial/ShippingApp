using Azure.Core;
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
        private readonly IAPIGatewayService _gatewayService;
        Response response = new Response();
        public DriverService(ShippingDbContext dbContext,IRabbitMQProducer rabbitMQProducer,IAPIGatewayService gatewayService)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
            _gatewayService = gatewayService;
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

        public Response GetShippers(Guid checkpointLocation)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            if (checkpointLocation == Guid.Empty)
            {
                response.Message = "Please enter the location";
                return response;
            }
            var shippers = _dbContext.Shippers.Where(s=>s.checkpoint1Id == checkpointLocation && s.isAccepted == false && s.isActive == true).Select(s=>s).ToList(); 
            if(shippers.Count == 0) {
                response.Data = null;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Don't have any shipments right now in your Location";
                return response;
            }
            response.Data = shippers;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Shipments";
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
            var obj = _dbContext.Shippers.Where(s => s.driverId == driver.driverId && s.checkpoint1Id == driver.checkpointLocation && s.isActive == true && s.isAccepted == true).Select(s => s);
            if (obj.Count() == 0)
            {
                driver.isAvailable = true;
                _dbContext.SaveChanges();
                response.Data = driver;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Driver location Updated";
                return response;
            }
            float totalWeight = 0;
            foreach (var shipment in obj)
            {
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            if(totalWeight < 17000)
            {
                response.Data = null;
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Fill the container atleast 70% to move on.";
                return response;
            }

            foreach (var item in obj)
            {
                var shipmentStatus = new ShipmentStatusModel()
                {
                    shipmentStatusId = Guid.NewGuid(),
                    shipmentId = item.shipmentId,
                    shipmentStatus = "Accepted",
                    currentLocation = updateDriver.checkpointLocation,
                    lastUpdated = DateTime.Now
                };
                item.isActive = false;
                _dbContext.SaveChanges();
                var shipmentRoute = _gatewayService.GetCheckpoints(item.shipmentId);
                int len = shipmentRoute.Count;
                int i = 0;
                int index = 0;
                foreach (var checkId in shipmentRoute)
                {
                    i++;
                    if (updateDriver.checkpointLocation == checkId.checkpointId)
                    {
                        index = i;
                        break;
                    }
                }
                var shipper = new ShippmentDriverMapping()
                {
                    mapId = Guid.NewGuid(),
                    shipmentId = item.shipmentId,
                    productType = item.productType,
                    containerType = item.containerType,
                    shipmentWeight = item.shipmentWeight,
                    isAccepted = false,
                    isActive = true,
                    driverId = Guid.Empty,
                    checkpoint1Id = updateDriver.checkpointLocation,
                    checkpoint2Id = shipmentRoute[index].checkpointId,
                };
                if (updateDriver.checkpointLocation == shipmentRoute[0].checkpointId)
                {
                    shipmentStatus.shipmentStatus = "Picked Up";
                }
                else if (updateDriver.checkpointLocation == shipmentRoute[len - 1].checkpointId)
                {
                    shipmentStatus.shipmentStatus = "Delivered";
                }
                else
                {
                    shipmentStatus.shipmentStatus = "In Transit";
                }

                _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                _dbContext.Shippers.Add(shipper);
                driver.isAvailable = false;
                _dbContext.SaveChanges();
            }
            response.Data = driver;
            response.StatusCode = 200;
            response.IsSuccess = true;  
            response.Message = "Driver Location Updated";
            return response;
        }
        public Response AcceptShipment(AcceptShipmentRequest request)
        {
            response.Data = null;
            response.IsSuccess = true;
            response.StatusCode = 200;
            if (request.mapId == Guid.Empty)
            {
                response.Message = "Please enter the MapId";
                return response;
            }
            if (request.driverId == Guid.Empty)
            {
                response.Message = "Please enter the DriverId";
                return response;
            }
            var shipper = _dbContext.Shippers.Where(s => s.mapId == request.mapId).FirstOrDefault();
            if (shipper == null)
            {
                response.Message = "Shipment does not exist";
                return response;
            }
            var driver = _dbContext.Drivers.Where(d => d.driverId == request.driverId).FirstOrDefault();
            var hasShipment = _dbContext.Shippers.Where(s => s.driverId == request.driverId && s.checkpoint1Id== driver.checkpointLocation && s.isActive == true && s.containerType == shipper.containerType && s.isAccepted == true).Select(s=>s);
            float totalWeight = 0;
            foreach (var shipment in hasShipment) {
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            if(hasShipment.Count() == 0)
            {
                shipper.driverId = request.driverId;
                shipper.isAccepted = request.isAccepted;
                driver.isAvailable = true;
                _dbContext.SaveChanges();
                response.Data = shipper;
                response.Message = "shipment Accepted";
                response.IsSuccess = true;
                response.StatusCode = 200;
                return response;
            }
            if(shipper.shipmentWeight + totalWeight <= 25000)
            {
                shipper.driverId = request.driverId;
                shipper.isAccepted = request.isAccepted;
                driver.isAvailable = true;
                _dbContext.SaveChanges();
                response.Data = shipper;        
                response.Message = "shipment Accepted";
                response.IsSuccess = true;
                response.StatusCode = 200;
                return response;
            }
            if(shipper.shipmentWeight + totalWeight > 25000)
            {
                response.Data = shipper;
                response.Message = "Cannot Accept the shipment. Container is full.";
                response.IsSuccess = false;
                response.StatusCode = 400;              
            }
            return response;
        }
    }
}

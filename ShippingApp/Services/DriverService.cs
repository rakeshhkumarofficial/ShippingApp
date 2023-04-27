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
        public Response GetDriver(Guid driverId, Guid checkpointLocation)
        {
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "Drivers List";
            var drivers = _dbContext.Drivers.Where(d => (d.driverId == driverId || driverId == Guid.Empty) && (d.checkpointLocation == checkpointLocation || checkpointLocation == Guid.Empty)).Select(d => d).ToList();
            if(drivers.Count == 0)
            {
                response.IsSuccess = false;
                response.StatusCode = 404;
                response.Message = "Drivers Not Found";
                response.Data = null;
                return response;
            }
            response.Data = drivers;
            return response;
        }

        public Response GetShippers(Guid checkpointLocation,Guid driverId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            if (driverId != Guid.Empty)
            {
                var shippersDriver = _dbContext.Shippers.Where(s => s.driverId == driverId).Select(s => s).ToList();
                List<GetShippersResponse> Shippers = new List<GetShippersResponse>();
                foreach (var shipper in shippersDriver)
                {
                    var checkpointLocation1 = _gatewayService.GetCheckpoints(shipper.checkpoint1Id);
                    var checkpointLocation2 = _gatewayService.GetCheckpoints(shipper.checkpoint2Id);
                    GetShippersResponse response = new GetShippersResponse()
                    {
                        mapId = shipper.mapId,
                        shipmentId = shipper.shipmentId,
                        productType = shipper.productType,
                        containerType = shipper.containerType,
                        shipmentWeight = shipper.shipmentWeight,
                        isAccepted = shipper.isAccepted,
                        isActive = shipper.isActive,
                        driverId = shipper.driverId,
                        checkpoint1Id = checkpointLocation1[0].checkpointName,
                        checkpoint2Id = checkpointLocation2[0].checkpointName,
                        time = shipper.time,
                    };
                    Shippers.Add(response);
                }
                response.Data = Shippers;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Shipments";
                return response;
            }

            if (checkpointLocation == Guid.Empty)
            {
                response.Message = "Please enter the location";
                return response;
            }

            var shippers = _dbContext.Shippers.Where(s=>s.checkpoint1Id == checkpointLocation && s.isAccepted == false && s.isActive == true).Select(s=>s).ToList(); 
            if(shippers.Count == 0) {
                response.Data = null;
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = "Don't have any shipments right now in your Location";
                return response;
            }
            List<GetShippersResponse> ShipperList= new List< GetShippersResponse>();
            foreach(var shipper in shippers)
            {
                var checkpointLocation1 = _gatewayService.GetCheckpoints(shipper.checkpoint1Id);
                var checkpointLocation2 = _gatewayService.GetCheckpoints(shipper.checkpoint2Id);
                GetShippersResponse response = new GetShippersResponse()
                {
                    mapId = shipper.mapId,
                    shipmentId = shipper.shipmentId,
                    productType = shipper.productType,
                    containerType = shipper.containerType,
                    shipmentWeight = shipper.shipmentWeight,
                    isAccepted = shipper.isAccepted,
                    isActive = shipper.isActive,
                    driverId = shipper.driverId,
                    checkpoint1Id = checkpointLocation1[0].checkpointName,
                    checkpoint2Id = checkpointLocation2[0].checkpointName,
                    time = shipper.time,
                };
                ShipperList.Add(response);
            }

            response.Data = ShipperList;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Shipments";
            return response;
        }

        public Response UpdateDriver(UpdateDriverRequest updateDriver)
        {
            Console.WriteLine(1);
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            response.Message = "No driver Found";
            var driver = _dbContext.Drivers.Find(updateDriver.driverId);
            if (driver == null)
            {
                Console.WriteLine(2);
                response.Message = "Driver Not Found";
                return response;
            }
            var obj = _dbContext.Shippers.Where(s => s.driverId == driver.driverId && s.isActive == true && s.isAccepted == true).Select(s => s).ToList();
            Console.WriteLine(3);
            if (obj.Count == 0)
            {
                Console.WriteLine(4);
                if (updateDriver.checkpointLocation != Guid.Empty) { driver.checkpointLocation = updateDriver.checkpointLocation; }
                driver.isAvailable = true;
                _dbContext.SaveChanges();
                response.Data = driver;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Driver location Updated";
                return response;
            }
            if (updateDriver.checkpointLocation != obj.First().checkpoint2Id) 
            {
                response.StatusCode = 400;
                response.Message = "Location Doesn't match with next checkpoint ";
                return response;
            }
            Console.WriteLine(5);
            float totalWeight = 0;
            foreach (var shipment in obj)
            {
                Console.WriteLine(6);
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            Console.WriteLine(7);
            if (totalWeight < 17000)
            {
                Console.WriteLine(8);
                response.Data = null;
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Fill the container atleast 70% to move on.";
                return response;
            }
            Console.WriteLine(9);
            if (updateDriver.checkpointLocation != Guid.Empty) 
            { 
                Console.WriteLine(10);
                driver.checkpointLocation = updateDriver.checkpointLocation;
                foreach (var item in obj)
                {
                    Console.WriteLine(11);
                    var shipmentStatus = new ShipmentStatusModel()
                    {
                        shipmentStatusId = Guid.NewGuid(),
                        shipmentId = item.shipmentId,
                        shipmentStatus = "Accepted",
                        currentLocation = updateDriver.checkpointLocation,
                        lastUpdated = DateTime.Now
                    };
                    item.isActive = false;
                    //_dbContext.SaveChanges();
                    var shipmentRoute = _gatewayService.GetShipmentRoute(item.shipmentId);
                    int len = shipmentRoute.Count;
                    int i = 0;
                    int index = 0;
                    foreach (var checkId in shipmentRoute)
                    {
                        Console.WriteLine(checkId.checkpointName);
                        index++;
                        if (updateDriver.checkpointLocation == checkId.checkpointId)
                        {
                            Console.WriteLine(12);
                            i = index;
                        }
                    }
                    Console.WriteLine(i +" hii");
                    Console.WriteLine(13);
                    if (i != len)
                    {
                        ShippmentDriverMapping shipper = new ShippmentDriverMapping()
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
                            checkpoint2Id = shipmentRoute[i].checkpointId,
                            time = DateTime.Now
                        };

                        Console.WriteLine(14);
                        if (updateDriver.checkpointLocation == shipmentRoute[0].checkpointId)
                        {
                            Console.WriteLine(15);
                            shipmentStatus.shipmentStatus = "Picked Up";
                        }
                        else if (updateDriver.checkpointLocation == shipmentRoute[len - 1].checkpointId)
                        {
                            Console.WriteLine(16);
                            shipmentStatus.shipmentStatus = "Delivered";
                        }
                        else
                        {
                            Console.WriteLine(17);
                            shipmentStatus.shipmentStatus = "Arrived";
                        }
                        Console.WriteLine(18);

                        _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                        Console.WriteLine(19);
                        _dbContext.Shippers.Add(shipper);
                        Console.WriteLine(20);
                        //driver.isAvailable = false;
                        _dbContext.SaveChanges();
                        Console.WriteLine(21);
                    }
                    else
                    {
                        shipmentStatus.shipmentStatus = "Delivered";
                        _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                        _dbContext.SaveChanges();
                    }
                }
                response.Data = driver;
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Driver Location Updated";
                Console.WriteLine(22);
                return response;
               
            }

            Console.WriteLine(23);
            return response;
        }
        public Response AcceptShipment(AcceptShipmentRequest request)
        {
            response.Data = null;
            response.IsSuccess = false;
            response.StatusCode = 400;
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
            var driver = _dbContext.Drivers.Find(request.driverId);
            var hasShipment = _dbContext.Shippers.Where(s => ((s.driverId == request.driverId) && (s.checkpoint1Id == driver.checkpointLocation) && (s.isActive == true) && (s.isAccepted == true))).Select(s => s).ToList();
            if (hasShipment.Count == 0)
            {
                var shipmentStatus = new ShipmentStatusModel()
                {
                    shipmentStatusId = Guid.NewGuid(),
                    shipmentId = shipper.shipmentId,
                    shipmentStatus = "Accepted",
                    currentLocation = shipper.checkpoint1Id,
                    lastUpdated = DateTime.Now
                };
                _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                shipper.driverId = request.driverId;
                shipper.isAccepted = request.isAccepted;
                shipper.time = DateTime.Now;
                driver.isAvailable = true;
                _dbContext.SaveChanges();
                response.Data = shipper;
                response.Message = "shipment Accepted";
                response.IsSuccess = true;
                response.StatusCode = 200;
                return response;
            }
            if(hasShipment.First().containerType !=shipper.containerType)
            {
                response.Message = " Container Type Does Not Match";
                return response;
            }
            if (hasShipment.First().checkpoint2Id != shipper.checkpoint2Id)
            {
                response.Message = " Destination Does Not Match";
                return response;
            }
            float totalWeight = 0;
            foreach (var shipment in hasShipment) {
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            if(shipper.shipmentWeight + totalWeight <= 25000)
            {
                var shipmentStatus = new ShipmentStatusModel()
                {
                    shipmentStatusId = Guid.NewGuid(),
                    shipmentId = shipper.shipmentId,
                    shipmentStatus = "Accepted",
                    currentLocation = shipper.checkpoint1Id,
                    lastUpdated = DateTime.Now
                };
                _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                shipper.driverId = request.driverId;
                shipper.isAccepted = request.isAccepted;
                shipper.time = DateTime.Now;
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
                response.Data = null;
                response.Message = "Cannot Accept the shipment. Container is full.";
                response.IsSuccess = false;
                response.StatusCode = 400;              
            }
            return response;
        }
    }
}

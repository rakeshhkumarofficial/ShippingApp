using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;
using ShippingApp.RabbitMQ;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        // add a new driver
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
        // delete the driver
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
        // find driver by his location and driverId
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

        // get shipements available according to the drivers location
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
                    var checkpointLocation1 = _gatewayService.GetCheckpoints(shipper.checkpoint1Id,null);
                    var checkpointLocation2 = _gatewayService.GetCheckpoints(shipper.checkpoint2Id,null);
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
                var checkpointLocation1 = _gatewayService.GetCheckpoints(shipper.checkpoint1Id,null);
                var checkpointLocation2 = _gatewayService.GetCheckpoints(shipper.checkpoint2Id,null);
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

        // update driver location
        public Response UpdateDriver(UpdateDriverRequest updateDriver)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            response.Message = "No driver Found";
            var driver = _dbContext.Drivers.Find(updateDriver.driverId);
            if (driver == null)
            {
                response.Message = "Driver Not Found";
                return response;
            }
            // check if driver already having some active shipments
            var obj = _dbContext.Shippers.Where(s => s.driverId == driver.driverId && s.isActive == true && s.isAccepted == true).Select(s => s).ToList();
           
            // if shipments count = 0 then update only location else update all active shipment location also associated with driver
            if (obj.Count == 0)
            {
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
            float totalWeight = 0;
            foreach (var shipment in obj)
            {
                Console.WriteLine(6);
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            // if shipments weight is > 17000 , then only update the driver location
            if (totalWeight < 17000)
            {
                Console.WriteLine(8);
                response.Data = null;
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = "Fill the container atleast 70% to move on.";
                return response;
            }
            if (updateDriver.checkpointLocation != Guid.Empty) 
            {
                var trip = new Trip()
                {
                    tripId = Guid.NewGuid(),
                    driverId = driver.driverId,
                    checkpoint1Id = driver.checkpointLocation,
                    checkpoint2Id = driver.checkpointLocation,
                    Price = 0,
                    dateTime = DateTime.Now,
                };
                driver.checkpointLocation = updateDriver.checkpointLocation;
                trip.checkpoint2Id = driver.checkpointLocation;
                float distance = _gatewayService.GetCheckpointsDistance(trip.checkpoint1Id, trip.checkpoint2Id);
                var container = _dbContext.Shippers.Where(s => s.driverId == driver.driverId && s.isActive == true && s.isAccepted == true).FirstOrDefault(); 
                float containerPrice = _dbContext.ContainerTypes.Where(c=>c.containerName == container.containerType).FirstOrDefault().price;
                trip.Price = ((distance + (totalWeight * containerPrice))*5*0.15f);
                _dbContext.Trips.Add(trip);
                _dbContext.SaveChanges();
                foreach (var item in obj)
                {
                    // send the shipment status to s2 service using rabbitMQ
                    var shipmentStatus = new ShipmentStatusModel()
                    {
                        shipmentStatusId = Guid.NewGuid(),
                        shipmentId = item.shipmentId,
                        shipmentStatus = "Accepted",
                        currentLocation = updateDriver.checkpointLocation,
                        lastUpdated = DateTime.Now
                    };
                    item.isActive = false;
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
                            i = index;
                        }
                    }
                    // generate the new shipment delivery 
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
                            shipmentStatus.shipmentStatus = "Arrived";
                        }
                        _rabbitMQProducer.SendStatusMessage(shipmentStatus);
                        _dbContext.Shippers.Add(shipper);
                        _dbContext.SaveChanges();
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
                return response;
               
            }
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
            // check the container type of the previous shipments and new shipment
            if(hasShipment.First().containerType !=shipper.containerType)
            {
                response.Message = " Container Type Does Not Match";
                return response;
            }

            // check the checkoint2Id of the previous shipments and new shipment
            if (hasShipment.First().checkpoint2Id != shipper.checkpoint2Id)
            {
                response.Message = " Destination Does Not Match";
                return response;
            }
            float totalWeight = 0;
            foreach (var shipment in hasShipment) {
                totalWeight = totalWeight + shipment.shipmentWeight;
            }
            // if weight all preivious + new shipemnt is <=25000 then only accept the new shipment
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
        public Response GetDriverEarnings(Guid driverId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(driverId);
            if (driver == null)
            {
                response.Message = "Driver Not Found";
                return response;
            }
            // total earnings of driver
            var totalEarnings = _dbContext.Trips.Where(t => t.driverId == driverId).Select(t => t.Price).ToList();
            float _totalEarning = 0;
            int _totalTrip = 0;
            foreach (var t in totalEarnings)
            {
                _totalEarning = _totalEarning + t;
                _totalTrip++;
            }
            // monthly earnings of driver
            var monthlyEarnings = _dbContext.Trips.Where(t => t.driverId == driverId && ( t.dateTime.Month == DateTime.Now.Month)).Select(t => t.Price).ToList();
            float _monthlyEarning = 0;
            foreach (var m in monthlyEarnings)
            {
                _monthlyEarning = _monthlyEarning + m;
            }
            // daily earnings of driver
            var todayEarnings = _dbContext.Trips.Where(t => t.driverId == driverId && (t.dateTime.Day == DateTime.Now.Day)).Select(t => t.Price).ToList();
            float _todayEarning = 0;
            foreach (var td in todayEarnings)
            {
                _todayEarning = _todayEarning + td;
            }
            var trip = new GetTripResponse()
            {
                totalTrips = _totalTrip,
                totalEarnings = _totalEarning,
                monthlyEarning = _monthlyEarning,
                todayEarning = _todayEarning

            };
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = " Total earnings";
            response.Data = trip;
            return response;
        }
        public Response GetDateEarnings(Guid driverId , DateTime date1 , DateTime date2)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(driverId);
            if (driver == null)
            {
                response.Message = "Driver Not Found";
                return response;
            }
            
            // monthly earnings of driver
            var Earnings = _dbContext.Trips.Where(t => t.driverId == driverId && (t.dateTime.Date >= date1.Date && t.dateTime.Date <=date2.Date)).Select(t => t.Price).ToList();
            float _earning = 0;
            int _totalTrip = 0;
            
            foreach (var e in Earnings)
            {
                _earning = _earning + e;
                _totalTrip++;
            }
            // daily earnings of driver
            var trip = new GetTripResponse()
            {
                totalTrips = _totalTrip,
                totalEarnings = _earning,
                monthlyEarning = 0,
                todayEarning = 0
            };
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = " Total earnings";
            response.Data = trip;
            return response;
        }
        public Response GetChartEarnings(Guid driverId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var driver = _dbContext.Drivers.Find(driverId);
            if (driver == null)
            {
                response.Message = "Driver Not Found";
                return response;
            }
            List<float> chartList = new List<float>();
            for(int i =1; i<=12; i++)
            {
                var Earnings = _dbContext.Trips.Where(t => t.driverId == driverId && (t.dateTime.Year == DateTime.Now.Year && t.dateTime.Month == i)).Select(t => t.Price).ToList();
                
                float _earning = 0;
                foreach (var e in Earnings)
                {
                    _earning = _earning + e;
                }
                
                chartList.Add(_earning);
            }
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = " Monthly chart earnings";
            response.Data = chartList;
            return response;
        }
    }
}

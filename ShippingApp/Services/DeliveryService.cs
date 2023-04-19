using ShippingApp.Data;
using ShippingApp.Models;
using ShippingApp.RabbitMQ;

namespace ShippingApp.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ShippingDbContext _dbContext;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        Response response = new Response();
        public DeliveryService(ShippingDbContext dbContext, IRabbitMQProducer rabbitMQProducer)
        {
            _dbContext = dbContext;
            _rabbitMQProducer = rabbitMQProducer;
        }

        

        public Response AddDelivery(ShipmentDeliveryModel shipmentDelivery)
        {
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Delivery Service Added";
            response.Data = shipmentDelivery;
            var driver = _dbContext.Drivers.Where(d => (d.checkpointLocation == shipmentDelivery.shipment.origin) && d.isAvailable == true).FirstOrDefault();
            if(driver == null)
            {
                response.Data = null;
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.Message = " No Driver is avialable";
                return response;
            }
            var shipper = new ShippmentDriverMapping()
            {
                mapId = Guid.NewGuid(),
                shipmentId = shipmentDelivery.shipment.shipmentId,
                driverId = driver.driverId,
                checkpoint1Id = shipmentDelivery.shipment.origin,
                checkpoint2Id = shipmentDelivery.shipment.destination,
            };
            var shipmentStatus = new ShipmentStatusModel()
            {
                shipmentStatusId = Guid.NewGuid(),
                shipmentId = shipper.shipmentId,
                shipmentStatus = "Accepted",
                currentLocation = shipper.checkpoint1Id,
                lastUpdated = DateTime.Now
            };
            var notifyDriver = new NotifyDriver()
            {
                driverId = driver.driverId,
                shipmentId = shipper.shipmentId
            };
            _rabbitMQProducer.SendProductMessage(shipmentStatus);
            _rabbitMQProducer.SendDriverMessage(notifyDriver);
            _dbContext.Shippers.Add(shipper);
            driver.isAvailable = false;
            _dbContext.SaveChanges();
            response.Data = shipper;
            return response;
        }
    }
}

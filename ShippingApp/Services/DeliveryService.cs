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
            var drivers = _dbContext.Drivers.Where(d => d.isAvailable == true && d.checkpointLocation == shipmentDelivery.shipment.origin).Select(d => d.driverId).ToList();
            var shipper = new ShippmentDriverMapping()
            {
                mapId = Guid.NewGuid(),
                shipmentId = shipmentDelivery.shipment.shipmentId,
                productType = shipmentDelivery.shipment.productType,
                containerType = shipmentDelivery.shipment.containerType,
                shipmentWeight = shipmentDelivery.shipment.shipmentWeight,
                isAccepted = false,
                isActive = true,
                driverId = Guid.Empty,
                checkpoint1Id = shipmentDelivery.shipment.origin,
                checkpoint2Id = shipmentDelivery.checkpoints[1].checkpointId,
                time = DateTime.Now            
            };
            
            _dbContext.Shippers.Add(shipper);
            _dbContext.SaveChanges();
            var notifyDriver = new NotifyDriver()
            {
                driverIds = drivers,
            };
            _rabbitMQProducer.SendDriverMessage(notifyDriver);
            response.Data = shipper;
            return response;
        }
    }
}

using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ShippingDbContext _dbContext;
        Response response = new Response();
        public DeliveryService(ShippingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Response AddDelivery(ShipmentModel shipment)
        {
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Delivery Service Added";
            var driver = _dbContext.Drivers.Where(d => (d.location.ToLower() == shipment.origin.ToLower()) && d.isAvailable == true).FirstOrDefault();
            var checkpoint1 = _dbContext.Checkpoints.Where(c=>c.location.ToLower() == shipment.origin.ToLower()).FirstOrDefault();
            var checkpoint2 = _dbContext.Checkpoints.Where(c => c.location.ToLower() == shipment.destination.ToLower()).FirstOrDefault();
            var shipper = new ShippmentDriverMapping()
            {
                mapId = Guid.NewGuid(),
                shipmentId = shipment.shipmentId,
                driverId = driver.driverId,
                checkpoint1Id = checkpoint1.checkpointId,
                checkpoint2Id = checkpoint2.checkpointId
            };
            _dbContext.Shippers.Add(shipper);
            driver.isAvailable = false;
            _dbContext.SaveChanges();
            response.Data = shipment;
            return response;
        }
    }
}

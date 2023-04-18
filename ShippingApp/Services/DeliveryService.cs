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
                checkpoint1Id = shipmentDelivery.checkpoints[0].checkpointId,
                checkpoint2Id = shipmentDelivery.checkpoints[1].checkpointId,
            };
            _dbContext.Shippers.Add(shipper);
            driver.isAvailable = false;
            _dbContext.SaveChanges();
            response.Data = shipper;
            return response;
        }
    }
}

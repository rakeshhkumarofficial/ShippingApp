using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IAPIGatewayService _gatewayService;

        public DriverController(IDriverService driverService, IAPIGatewayService gatewayService)
        {
            _driverService = driverService;
            _gatewayService = gatewayService;
        }
        [HttpPost]
        public ActionResult Add(Driver driver)
        {
            var res = _driverService.AddDriver(driver);
            return Ok(res);
        }

        [HttpDelete]
        public ActionResult Remove(Guid driverId)
        {
            var res = _driverService.DeleteDriver(driverId);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult Search(Guid driverId, Guid checkpointLocation , bool isAvailable)
        {
            var res = _driverService.GetDriver(driverId, checkpointLocation, isAvailable);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Update(UpdateDriverRequest updateDriver)
        {
            var res = _driverService.UpdateDriver(updateDriver);
            return Ok(res);
        }


        [HttpGet]
        public ActionResult GetShippers(Guid id)
        {
            GetShippersRequest temp = new GetShippersRequest(id);
            var res = _driverService.GetShippers(temp);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult AcceptShipment(AcceptShipmentRequest request)
        {
            var res = _driverService.AcceptShipment(request);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetShipmentRoute(Guid shipmentId)
        {          
            var res = _gatewayService.GetCheckpoints(shipmentId);
            return Ok(res);
        }

    }
}

﻿using Microsoft.AspNetCore.Http;
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
        public ActionResult Search(Guid driverId , Guid checkpointLocation)
        {
            var res = _driverService.GetDriver(driverId, checkpointLocation);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Update(UpdateDriverRequest updateDriver)
        {
            var res = _driverService.UpdateDriver(updateDriver);
            return Ok(res);
        }


        [HttpGet]
        public ActionResult GetShippers(Guid checkpointLocation, Guid driverId)
        {          
            var res = _driverService.GetShippers(checkpointLocation, driverId);
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
            var res = _gatewayService.GetShipmentRoute(shipmentId);
            return Ok(res);
        }


        [HttpGet]
        public ActionResult GetCheckpoints(Guid checkpointId)
        {
            var res = _gatewayService.GetCheckpoints(checkpointId,null);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetCheckpointsDistance(Guid checkpoint1Id,Guid checkpoint2Id)
        {
            var res = _gatewayService.GetCheckpointsDistance(checkpoint1Id, checkpoint2Id);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetDriverEarnings(Guid driverId)
        {
            var res = _driverService.GetDriverEarnings(driverId);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetDateEarnings(Guid driverId,DateTime date1 , DateTime date2)
        {
            var res = _driverService.GetDateEarnings(driverId,date1,date2);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetChartEarnings(Guid driverId)
        {
            var res = _driverService.GetChartEarnings(driverId);
            return Ok(res);
        }

    }
}

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

        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
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
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerTypeController : ControllerBase
    {
        private readonly IContainerTypeService _conatinerTypeService;

        public ContainerTypeController(IContainerTypeService conatinerTypeService)
        {
            _conatinerTypeService = conatinerTypeService;
        }
        [HttpPost]
        public ActionResult Add(AddContainerTypeRequest addContainerTypeRequest)
        {
            var res = _conatinerTypeService.AddContainerType(addContainerTypeRequest);
            return Ok(res);
        }
    }
}

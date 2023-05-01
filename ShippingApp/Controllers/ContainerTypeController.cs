using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ContainerTypeController : ControllerBase
    {
        private readonly IContainerTypeService _containerTypeService;
        public ContainerTypeController(IContainerTypeService containerTypeService)
        {
            _containerTypeService = containerTypeService;
        }
        [HttpPost]
        public ActionResult Add(AddContainerTypeRequest addContainerTypeRequest)
        {
            var res = _containerTypeService.AddContainerType(addContainerTypeRequest);
            return Ok(res);
        }

        [HttpDelete]
        public ActionResult Remove(Guid containerTypeId)
        {
            var res = _containerTypeService.DeleteContainerType(containerTypeId);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult Search(Guid containerTypeId, string? searchString)
        {
            var res = _containerTypeService.GetContainerTypes(containerTypeId, searchString);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Update(ContainerType updateContainer)
        {
            var res = _containerTypeService.UpdateContainerType(updateContainer);
            return Ok(res);
        }
    }
}

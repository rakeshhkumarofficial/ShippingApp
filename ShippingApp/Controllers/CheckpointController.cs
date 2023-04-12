using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CheckpointController : ControllerBase
    {
        private readonly ICheckpointService _checkpointService;

        public CheckpointController(ICheckpointService checkpointService)
        {
            _checkpointService = checkpointService;
        }
        [HttpPost]
        public ActionResult Add(AddCheckpointRequest addCheckpoint)
        {
            var res = _checkpointService.AddCheckpoint(addCheckpoint);
            return Ok(res);
        }

        [HttpDelete]
        public ActionResult Remove(Guid checkpointId)
        {
            var res = _checkpointService.DeleteCheckpoint(checkpointId);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult Search(Guid checkpointId, string? location)
        {
            var res = _checkpointService.GetCheckpoints(checkpointId, location);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Update(Checkpoint updateCheckpoint)
        {
            var res = _checkpointService.UpdateCheckpoint(updateCheckpoint);
            return Ok(res);
        }
    }
}

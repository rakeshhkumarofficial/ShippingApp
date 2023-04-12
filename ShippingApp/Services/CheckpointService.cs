
using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class CheckpointService : ICheckpointService
    {
        private readonly ShippingDbContext _dbContext;
        Response response = new Response();
        public CheckpointService(ShippingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Response AddCheckpoint(AddCheckpointRequest addCheckpoint)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            if (addCheckpoint.location == null || addCheckpoint.location == "")
            {
                response.Message = "Please Enter the location";
                return response;
            }
            var isExists = _dbContext.Checkpoints.Where(u => u.location.ToLower() == addCheckpoint.location.ToLower());
            if (isExists.Any())
            {
                response.Message = "This Checkpoint already exists";
                return response;
            }
            var obj = new Checkpoint()
            {
                checkpointId = Guid.NewGuid(),
                location = addCheckpoint.location,
            };
            _dbContext.Checkpoints.Add(obj);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Data = obj;
            response.Message = "New Checkpoint Created";
            return response;
        }

        public Response DeleteCheckpoint(Guid checkpointId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var checkpoint = _dbContext.Checkpoints.Find(checkpointId);
            if (checkpoint == null)
            {
                response.Message = "Checkpoint Not found";
                return response;
            }
            _dbContext.Checkpoints.Remove(checkpoint);
            _dbContext.SaveChanges();
            response.Data = checkpoint;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Checkpoint is Deleted";
            return response;
        }
        public Response GetCheckpoints(Guid checkpointId, string? location)
        {
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "Checkpoint List";
            if (checkpointId == Guid.Empty && location == null)
            {
                var obj = _dbContext.Checkpoints;
                response.Data = obj;
                return response;
            }

            var checkpoints = from checkpoint in _dbContext.Checkpoints where ((checkpoint.checkpointId == checkpointId || checkpointId == Guid.Empty) && (EF.Functions.Like(checkpoint.location, "%" + location + "%") || location == null)) select checkpoint;
            response.Data = checkpoints;
            return response;
        }
        public Response UpdateCheckpoint(Checkpoint updateCheckpoint)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var checkpoint = _dbContext.Checkpoints.Find(updateCheckpoint.checkpointId);
            if (checkpoint == null)
            {
                response.Message = "Checkpoint not Found";
                return response;
            }
            if (updateCheckpoint.location != null) { checkpoint.location = updateCheckpoint.location; }
            _dbContext.SaveChanges();
            response.Data = checkpoint;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Checkpoint is Updated";
            return response;
        }
    }
}

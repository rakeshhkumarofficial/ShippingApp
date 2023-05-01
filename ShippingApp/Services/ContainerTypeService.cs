
using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;


namespace ShippingApp.Services
{
    public class ContainerTypeService : IContainerTypeService
    {
        private readonly ShippingDbContext _dbContext;
        Response response = new Response();
        public ContainerTypeService(ShippingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // add a new container type
        public Response AddContainerType(AddContainerTypeRequest addContainer)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            if (addContainer.containerName == null || addContainer.containerName == "" || addContainer == null)
            {
                response.Message = "Please Enter the Container Type";
                return response;
            }
            if(addContainer.price == 0)
            {
                response.Message = "Please Enter the Container Price";
                return response;
            }
          
            var obj = new ContainerType()
            {
                containerTypeId = Guid.NewGuid(),
                containerName = addContainer.containerName,
                price = addContainer.price
            };
            _dbContext.ContainerTypes.Add(obj);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Data = obj;
            response.Message = "New Container Type Created";
            return response;
        }

        // delete a container type
        public Response DeleteContainerType(Guid containerTypeId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var containerType = _dbContext.ContainerTypes.Find(containerTypeId);
            if (containerType == null)
            {
                response.Message = "Container Type Not Exists";
                return response;
            }
            _dbContext.ContainerTypes.Remove(containerType);
            _dbContext.SaveChanges();
            response.Data = containerType;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Container Type is Deleted";
            return response;
        }

        // get a container type
        public Response GetContainerTypes(Guid containerTypeId, string? containerName)
        {
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "Container Type List";
            if (containerTypeId == Guid.Empty && containerName == null)
            {
                var obj = _dbContext.ContainerTypes;
                response.Data = obj;
                return response;
            }
            var containerTypes = from containerType in _dbContext.ContainerTypes where ((containerType.containerTypeId == containerTypeId || containerTypeId == Guid.Empty) && (EF.Functions.Like(containerType.containerName, "%" + containerName + "%") || containerName == null)) select containerType;
            response.Data = containerTypes;
            return response;
        }
        // update a container type
        public Response UpdateContainerType(ContainerType updateContainer)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var containerType = _dbContext.ContainerTypes.Find(updateContainer.containerTypeId);
            if (containerType == null)
            {
                response.Message = "Container Type Not Exists";
                return response;
            }
            if (updateContainer.containerName != null) { containerType.containerName = updateContainer.containerName; }
            if (updateContainer.price != -1 || updateContainer.price != 0) { containerType.price = updateContainer.price; }
            _dbContext.SaveChanges();
            response.Data = containerType;
            response.StatusCode = 200;
            response.Message = "Container Type Updated";
            return response;
        }
    }
}

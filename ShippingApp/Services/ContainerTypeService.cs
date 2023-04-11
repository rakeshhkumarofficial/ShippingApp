
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

        public Response AddContainerType(AddContainerTypeRequest addContainer)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            if (addContainer.containerName == null || addContainer.containerName == "" || addContainer == null)
            {
                response.Message = "Please Enter the Conatiner Type";
                return response;
            }
          
            var obj = new ContainerType()
            {
                containerTypeId = Guid.NewGuid(),
                containerName = addContainer.containerName
            };
            _dbContext.ContainerTypes.Add(obj);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Data = obj;
            response.Message = "New Conatiner Type Created";
            return response;
        }
    }
}

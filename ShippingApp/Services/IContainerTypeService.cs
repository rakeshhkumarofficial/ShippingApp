using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IContainerTypeService
    {
        public Response AddContainerType(AddContainerTypeRequest addContainer);
        public Response DeleteContainerType(Guid containerTypeId);
        public Response GetContainerTypes(Guid containerTypeId, string? containerName);
        public Response UpdateContainerType(ContainerType updateContainer);
    }
}

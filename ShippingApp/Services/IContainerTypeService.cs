using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IContainerTypeService
    {
        public Response AddContainerType(AddContainerTypeRequest addContainer);
    }
}

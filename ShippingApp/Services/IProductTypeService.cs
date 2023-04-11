using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IProductTypeService
    {
        public Response AddProductType(AddProductTypeRequest productType);
        public Response DeleteProductType(Guid productTypeId);
        public Response GetProductTypes(Guid productTypeId, string? type);
        public Response UpdateProductType(ProductType updateType);

    }
}

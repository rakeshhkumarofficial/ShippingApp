using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }
        [HttpPost]
        public ActionResult Add(AddProductTypeRequest addProductTypeRequest)
        {
            var res = _productTypeService.AddProductType(addProductTypeRequest);
            return Ok(res);
        }

        [HttpDelete]
        public ActionResult Remove(Guid productTypeId)
        {
            var res = _productTypeService.DeleteProductType(productTypeId);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult Search(Guid productTypeId, string? searchString)
        {
            var res = _productTypeService.GetProductTypes(productTypeId, searchString);
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Update(ProductType updateType)
        {
            var res = _productTypeService.UpdateProductType(updateType);
            return Ok(res);
        }
    }
}

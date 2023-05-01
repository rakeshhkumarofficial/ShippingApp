using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Models;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShippingApp.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ShippingDbContext _dbContext;
        Response response = new Response();

        public ProductTypeService(ShippingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // add a new product type
        public Response AddProductType(AddProductTypeRequest productType)
        {
            response.Data = null;
            response.StatusCode = 400;
            response.IsSuccess = false;
            if (productType.type == null || productType.type == "" || productType == null)
            {
                response.Message = "Please Enter the Product Type";
                return response;
            }
            if (productType.price == 0)
            {
                response.Message = "Please Enter the Price";
                return response;
            }
            var obj = new ProductType()
            {
                productTypeId = Guid.NewGuid(),
                type = productType.type,
                price = productType.price,
                isFragile = productType.isFragile
            };
            _dbContext.ProductTypes.Add(obj);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Data = obj;
            response.Message = "New Product Type Created";
            return response;
        }

        // delete a existing product type
        public Response DeleteProductType(Guid productTypeId)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var productType = _dbContext.ProductTypes.Find(productTypeId);
            if(productType == null) {
                response.Message = "Product Type Not Exists";
                return response;
            }
            _dbContext.ProductTypes.Remove(productType);
            _dbContext.SaveChanges();
            response.Data = productType;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Product Type is Deleted";
            return response;
        }
        // get a product type
        public Response GetProductTypes(Guid productTypeId, string? type)
        {
            response.IsSuccess = true;
            response.StatusCode = 200;
            response.Message = "Product Type List";
            if (productTypeId == Guid.Empty && type == null)
            {
                var obj = _dbContext.ProductTypes; 
                response.Data = obj;
                return response;
            }          
            var productTypes = from productType in _dbContext.ProductTypes where ((productType.productTypeId == productTypeId || productTypeId == Guid.Empty) && (EF.Functions.Like(productType.type, "%" + type + "%")|| type == null)) select productType;
            response.Data = productTypes;
            return response;
        }

        // update  a existing product type
        public Response UpdateProductType(ProductType updateType)
        {
            response.Data = null;
            response.StatusCode = 404;
            response.IsSuccess = false;
            var productType = _dbContext.ProductTypes.Find(updateType.productTypeId);
            if (productType == null)
            {
                response.Message = "Product Type Not Exists";
                return response;
            }
            if (updateType.type != null) { productType.type = updateType.type; }
            if (updateType.price != -1 || updateType.price !=0 ) { productType.price = updateType.price; }
            productType.isFragile = updateType.isFragile;
            _dbContext.SaveChanges();
            response.Data = productType;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "Product Type Updated";
            return response;
        }
    }
}

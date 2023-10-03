using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Entities.Generic;
using Ecommerce.Presentation.Extensions;
using Ecommerce.Service;
using Ecommerce.Service.Extensions;
using Ecommerce.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Presentation.ActionFilters;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Utils;
using Ecommerce.Presentation.Infrastructure.Extensions;

namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        public readonly ProductRepository repository;

        public ProductController(ProductRepository repository)
        {
            this.repository = repository;

        }


        [HttpGet]
        public IActionResult GetProducts([FromQuery] ProductsParameters parameters)
        {
            var page = parameters.PageNumber;
            var pageSize = parameters.PageSize;
            var skipCount = (page - 1) * pageSize;

            var filteredProducts = repository.GetAll()
                                   .Where(product =>
                                    (parameters.MinPrice <= product.Price) &&
                                    (parameters.MaxPrice >= product.Price) &&
                                    (parameters.AddedOn == DateTime.MinValue || parameters.AddedOn == product.AddedOn) &&
                                    (string.IsNullOrEmpty(parameters.Sku) || parameters.Sku == product.SKU))
                                    .ToList();
            var totalItemCount = filteredProducts.Count;

            var metadata = new MetaData().Initialize(page, pageSize, totalItemCount);
            metadata.AddResponseHeaders(Response);
            var pagedList = PagedList<Product>.ToPagedList(filteredProducts, page, pageSize);
            return Ok(pagedList);
        }


        [HttpGet("{id}")]
        public IActionResult GetProduct(Guid id)
        {
            var rtval = repository.GetById(id);
            return Ok(rtval);
        }


        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult AddProduct([FromBody] ProductDto dto)
        {
            var product = dto.MaptoProduct();
            var rtval = repository.Add(product);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(rtval);
        }


        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult UpdateProduct([FromBody] ProductDto dto)
        {
            var product = dto.MaptoProduct();
            var rtval = repository.Update(product);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(rtval);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var product = repository.GetById(id);   
            repository.Remove(product);
            repository.Save();
            var apimodel = new ApiResponseModel<Product>(ApiResponseStatusEnum.Success,
                    "Product Deleted Successfully", product);
            return Ok(apimodel);
        }
    }
}
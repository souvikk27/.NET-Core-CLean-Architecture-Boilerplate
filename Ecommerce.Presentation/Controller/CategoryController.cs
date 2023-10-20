using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Domain.Entities;
using Ecommerce.Presentation.ActionFilters;
using Ecommerce.Presentation.Extensions;
using Ecommerce.Presentation.Infrastructure.Extensions;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Utils;
using Ecommerce.Service;
using Ecommerce.Service.Extensions;
using Ecommerce.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.RateLimiting;

namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository repository;
        public CategoryController(CategoryRepository repository)
        {
            this.repository = repository;
        }
        

        [HttpGet]
        [RateLimit(5,5)]
        public IActionResult GetAllCategories([FromQuery] CategoryParameters parameters)
        {
            var page = parameters.PageNumber;
            var pageSize = parameters.PageSize;

            var filteredCategories = repository.GetAll()
                                     .Where(category => 
                                     (parameters.AddedOn == DateTime.MinValue || parameters.AddedOn == category.AddedOn) &&
                                     (string.IsNullOrEmpty(parameters.CategoryName) || parameters.CategoryName == category.Name))
                                     .ToList();
            var totalItemCount = filteredCategories.Count;
            var metadata = new MetaData().Initialize(page, pageSize, totalItemCount);
            metadata.AddResponseHeaders(Response);

            var pagedList = PagedList<Category>.ToPagedList(filteredCategories, page, pageSize);
            return Ok(pagedList);
        }



        [HttpGet("{id}")]
        [RateLimit(20, 60)]
        public IActionResult GetCategoryById(Guid id)
        {
            var category = repository.GetById(id);
            return Ok(category);
        }



        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult AddCategory([FromBody] CategoryDto dto)
        {
            var category = dto.MaptoCategory();
            repository.Add(category);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(category);
        }


        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult UpdateCategory([FromBody] CategoryDto dto)
        {
            var category = dto.MaptoCategory();
            category.ModifiedOn = DateTime.Now;
            repository.Update(category);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(category, "Category updated successfully", "204");
        }


        [HttpDelete]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = repository.GetById(id);
            repository.Remove(category);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(category, "Category deleted successfully");
        }
    }
}
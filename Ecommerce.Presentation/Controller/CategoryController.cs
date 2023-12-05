using Ecommerce.Presentation.Infrastructure.Extensions;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Mapster;

namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository repository;
        public CategoryController(CategoryRepository repository)
        {
            this.repository = repository;
        }
        

        [HttpGet]
        [Authorize]
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
            var category = dto.Adapt<Category>();
            repository.Add(category);
            repository.Save();
            return ApiResponseExtension.ToSuccessApiResult(category);
        }


        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult UpdateCategory([FromBody] CategoryDto dto)
        {
            var category = dto.Adapt<Category>();
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
using Ecommerce.Presentation.Infrastructure.Extensions;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Utils;


namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/category")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository repository;
        private readonly ApplicationContext _context; 
        public CategoryController(CategoryRepository repository, ApplicationContext context)
        {
            this.repository = repository;
            _context = context;
        }


        [HttpGet]
        public IActionResult GetAllCategories([FromQuery] CategoryParameters parameters)
        {
            var page = parameters.PageNumber;
            var pageSize = parameters.PageSize;

            var filteredCategories = repository.GetAll()
                                     .Where(category =>
                                     (parameters.AddedOn == DateTime.MinValue || parameters.AddedOn == category.CreatedAt) &&
                                     (string.IsNullOrEmpty(parameters.CategoryName) || parameters.CategoryName == category.CategoryName))
                                     .ToList();
            var totalItemCount = filteredCategories.Count;
            var metadata = new MetaData().Initialize(page, pageSize, totalItemCount);
            metadata.AddResponseHeaders(Response);

            var pagedList = PagedList<Category>.ToPagedList(filteredCategories, page, pageSize);
            return Ok(pagedList);
        }

        [HttpGet]
        [Route("subcategories")]
        public IActionResult GetSubcategories()
        {
            var subcategories = repository.GetCategoriesWithChildrenJson();
            return Ok(subcategories);
        }


        [HttpGet("{id}")]
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
            category.UpdatedAt = DateTime.Now;
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
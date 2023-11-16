using Ecommerce.Presentation.ActionFilters;
using Ecommerce.Presentation.Extensions;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Service;
using Ecommerce.Service.Extensions;
using Ecommerce.Shared.DTO;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository repository;
        public AdminController(UserRepository repository)
        {
            this.repository = repository;
            
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser(UserDto dto)
        {
            var user = dto.MaptoUser();
            var response = await repository.CreateUser(user, dto.Password);
            if(response == null)
            {
                return ApiResponseExtension.ToErrorApiResult(dto.UserName, "User Already Exists");
            }
            return ApiResponseExtension.ToSuccessApiResult(response);
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> GetToken([FromQuery] AuthParameters auth)
        {
            var token = await repository.GetTokenAsync(auth.Client_ID, auth.Client_Secret, auth.Refresh_Token);
            return Ok(token);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await repository.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await repository.GetById(id);
            if(user == null)
            {
                return ApiResponseExtension.ToErrorApiResult(id, "User does not exist", "404");
            }
            return ApiResponseExtension.ToSuccessApiResult(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await repository.Delete(id);

            if(user == null)
            {
                return ApiResponseExtension.ToErrorApiResult(id, "User does not exist", "404");
            }

            return ApiResponseExtension.ToSuccessApiResult(user, "User credentials removed");
        }
    }
}

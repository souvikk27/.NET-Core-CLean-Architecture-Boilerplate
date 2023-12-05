using System;
using Ecommerce.Presentation.Infrastructure.Filtering;



namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository repository;
        private readonly IOpenIddictApplicationManager _applicationManager;
        
        public AdminController(UserRepository repository, IOpenIddictApplicationManager applicationManager)
        {
            this.repository = repository;
            _applicationManager = applicationManager;
        }
        
        [HttpPost]
        [Route("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser([FromBody]UserDto dto)
        {
            var user = dto.Adapt<ApplicationUser>();
            var cancellationToken = new CancellationTokenSource().Token;
            if (user == null) return ApiResponseExtension.ToErrorApiResult(dto, "User parameters required");
            var response = await repository.CreateUser(user, dto.Password);
            return ApiResponseExtension.ToSuccessApiResult(user);
        }
        
        
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> GetToken([FromBody] AuthParameters auth)
        {
            var token = await repository.GetTokenAsync(auth.client_ID, auth.client_Secret);
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
            return ApiResponseExtension.ToSuccessApiResult(user);
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await repository.Delete(id);
            return ApiResponseExtension.ToSuccessApiResult(user, "User credentials removed");
        }


        
    }
}

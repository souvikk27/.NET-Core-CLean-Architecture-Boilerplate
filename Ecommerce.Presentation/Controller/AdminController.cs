using System;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Presentation.Infrastructure.Services.Abstraction;



namespace Ecommerce.Presentation.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository repository;
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IClientCredentialService _clientCredentialService;

        public AdminController(UserRepository repository, IOpenIddictApplicationManager applicationManager
            , IClientCredentialService clientCredentialService)
        {
            this.repository = repository;
            _applicationManager = applicationManager;
            _clientCredentialService = clientCredentialService;
        }

        [HttpPost]
        [Route("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddUser([FromBody] UserDto dto)
        {
            var user = dto.Adapt<ApplicationUser>();
            var cancellationToken = new CancellationTokenSource().Token;
            if (user == null) return ApiResponseExtension.ToErrorApiResult(dto, "User parameters required");

            var invokeClient = await _clientCredentialService.InvokeCredentialsAsync();

            var response = await repository.CreateUser(user, dto.Password, invokeClient);
            return ApiResponseExtension.ToSuccessApiResult(user);
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

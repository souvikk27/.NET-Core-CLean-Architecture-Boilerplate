using System.Security.Claims;
using Ecommerce.Presentation.ActionFilters;
using Ecommerce.Presentation.Infrastructure.Filtering;
using Ecommerce.Service;
using Ecommerce.Service.Extensions;
using Ecommerce.Shared.DTO;
using Ecommerce.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;


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
            var user = dto.Adapt<ApplicationUser>();
            var response = await repository.CreateUser(user, dto.Password);
            if(response == null)
            {
                return ApiResponseExtension.ToErrorApiResult(dto.UserName, "User Already Exists");
            }
            return ApiResponseExtension.ToSuccessApiResult(response);
        }

        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public IActionResult Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            ClaimsPrincipal claimsPrincipal;

            if (request.IsClientCredentialsGrantType())
            {
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

                // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
                identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

                claimsPrincipal = new ClaimsPrincipal(identity);

                claimsPrincipal.SetScopes(request.GetScopes());
            }
            else
            {
                throw new InvalidOperationException("The specified grant type is not supported.");
            }
            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        
        // [HttpPost]
        // [Route("token")]
        // public async Task<IActionResult> GetToken([FromQuery] AuthParameters auth)
        // {
        //     var token = await repository.GetTokenAsync(auth.Client_ID, auth.Client_Secret, auth.Refresh_Token);
        //     return Ok(token);
        // }

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

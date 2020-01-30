using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class UserController : SocnetoController
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/user/login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest login)
        {
            var authenticatedUser = await _userService.Authenticate(login.Username, login.Password);
            
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Username or password is incorrect" });

            var loginResponse = LoginResponse.FromModel(authenticatedUser);
            return Ok(loginResponse);
        }

    }
}
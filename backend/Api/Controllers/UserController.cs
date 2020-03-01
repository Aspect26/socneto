using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models.Requests;
using Socneto.Api.Models.Responses;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class UserController : SocnetoController
    {
        
        private readonly IUserService _userService;
        private readonly IEventTracker<UserController> _eventTracker;


        public UserController(IUserService userService, IEventTracker<UserController> eventTracker)
        {
            _userService = userService;
            _eventTracker = eventTracker;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/user/login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest login)
        {
            _eventTracker.TrackInfo("Login", "A user tried to login", new { User = login.Username });
            var authenticatedUser = await _userService.Authenticate(login.Username, login.Password);

            if (authenticatedUser == null)
            {
                _eventTracker.TrackInfo("Login", "Wrong credentials when trying to login");
                return Unauthorized(new {message = "Username or password is incorrect"});
            }

            var loginResponse = LoginResponse.FromModel(authenticatedUser);
            return Ok(loginResponse);
        }

    }
}
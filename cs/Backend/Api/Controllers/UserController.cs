using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Extensions;
using Socneto.Api.Helpers;
using Socneto.Api.Models;
using Socneto.Domain;
using Socneto.Domain.QueryResult;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IQueryUserJobService _queryUserJobService;
        private readonly IUserService _userService;


        public UserController(IQueryUserJobService queryUserJobService, IUserService userService)
        {
            _queryUserJobService = queryUserJobService;
            _userService = userService;
        }

        [HttpGet]
        [Route("api/user/{userId}/jobs")]
        public async Task<ActionResult<List<JobStatusResponse>>> GetJobStatuses([FromRoute]int userId)
        {
            if (!IsAuthorized(userId))
                return Unauthorized();
            
            var jobStatuses = await _queryUserJobService.GetJobStatuses(userId);

            var mappedJobStatuses = jobStatuses
                .Select(JobStatusResponse.FromModel)
                .ToList();
            
            return Ok(mappedJobStatuses);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/user/login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest login)
        {
            var authenticatedUser = await _userService.Authenticate(login.Username, login.Password);
            
            if (authenticatedUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var loginResponse = LoginResponse.FromModel(authenticatedUser);
            return Ok(loginResponse);
        }
        
        private bool IsAuthorized(int userId)
        {
            return userId == User.GetUserId();
        }

    }
}
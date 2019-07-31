﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IUserService _userService;


        public UserController(IJobService jobService, IUserService userService)
        {
            _jobService = jobService;
            _userService = userService;
        }

        [HttpGet]
        [Route("api/user/{userId}/jobs")]
        public async Task<ActionResult<List<JobStatusResponse>>> GetJobStatuses([FromRoute]int userId)
        {
            if (! await IsAuthorizedToSeeUser(userId))
                return Unauthorized();
            
            var jobStatuses = await _jobService.GetJobStatuses(userId);

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
        
        private async Task<bool> IsAuthorizedToSeeUser(int userId)
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            
            var user = await _userService.GetUserByName(User.Identity.Name);
            return userId == user.Id;
        }

    }
}
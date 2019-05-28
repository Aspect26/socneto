using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain.QueryResult;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IQueryUserJobService _queryUserJobService;


        public UserController(IQueryUserJobService queryUserJobService)
        {
            _queryUserJobService = queryUserJobService;
        }

        [HttpGet]
        [Route("api/user/{userId}/jobs")]
        public async Task<ActionResult<List<JobStatusResponse>>> GetJobStatuses([FromRoute]int userId)
        {

            var jobStatused = await _queryUserJobService.GetJobStatuses(userId);

            var mappedJobStatuses = jobStatused
                .Select(JobStatusResponse.FromModel)
                .ToList();
            
            return Ok(mappedJobStatuses);
        }
    }
}
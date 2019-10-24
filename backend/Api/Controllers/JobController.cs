using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Services;

using DataPoint = System.Collections.Generic.IList<dynamic>;


namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly ILogger<JobController> _logger;


        public JobController(IJobService jobService, IStorageService storageService, ILogger<JobController> logger)
        {
            _jobService = jobService;
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/status")]
        public async Task<ActionResult<JobDto>> GetJobDetail([FromRoute]Guid jobId)
        {
            if (! await IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var jobDetail = await _jobService.GetJobDetail(jobId);

            var jobStatusResponse = JobDto.FromModel(jobDetail);
            return Ok(jobStatusResponse);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/posts")]
        public async Task<ActionResult<List<PostDto>>> GetJobPosts([FromRoute] Guid jobId)
        {
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var posts = await _jobService.GetJobPosts(jobId);

            var mappedPosts = posts.Select(PostDto.FromValue).ToList();
            return Ok(mappedPosts);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/analysis")]
        public async Task<ActionResult<Task<IList<IList<DataPoint>>>>> GetJobAnalysis([FromRoute]Guid jobId, [FromBody] CreateChartDefinitionRequest chartDefinition)
        {
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var data = await _storageService.GetAnalyses();
            return Ok(data);
        }

        private async Task<bool> IsAuthorizedToSeeJob(Guid jobId)
        {
            var job = await _storageService.GetJob(jobId);
            return job.Username == User.Identity.Name;
        }
    }
}

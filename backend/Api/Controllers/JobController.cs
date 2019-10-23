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
            _logger.LogInformation("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
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
            _logger.LogInformation("BBBBB");
            /*
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var data = await _storageService.GetAnalyses();
            */

            var now = DateTime.Now;
            var data = new List<List<DataPoint>>()
            {
                new List<DataPoint>()
                {
                    new List<dynamic>() { now.AddMinutes(-1), 1 },
                    new List<dynamic>() { now.AddMinutes(-2), 0 },
                    new List<dynamic>() { now.AddMinutes(-3), 0 },
                    new List<dynamic>() { now.AddMinutes(-4), 1 },
                    new List<dynamic>() { now.AddMinutes(-5), 0 }
                },
                
                new List<DataPoint>()
                {
                    new List<dynamic>() { now.AddMinutes(-1), 0.98 },
                    new List<dynamic>() { now.AddMinutes(-2), 0.95 },
                    new List<dynamic>() { now.AddMinutes(-3), 0.99 },
                    new List<dynamic>() { now.AddMinutes(-4), 0.93 },
                    new List<dynamic>() { now.AddMinutes(-5), 0.90 }
                },
            };
            
            _logger.LogInformation("AAAAAAAAAAAAAAA");
            
            return Ok(data);
        }

        private async Task<bool> IsAuthorizedToSeeJob(Guid jobId)
        {
            var job = await _storageService.GetJob(jobId);
            return job.Username == User.Identity.Name;
        }
    }
}

using System;
using System.Collections.Generic;
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
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;


        public JobController(IJobService jobService, IStorageService storageService)
        {
            _jobService = jobService;
            _storageService = storageService;
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
        
        [HttpGet]
        [Route("api/job/{jobId:guid}/analysis")]
        public async Task<ActionResult<List<AnalyzedPostDto>>> GetJobAnalysis([FromRoute]Guid jobId)
        {
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var analyzedPosts = await _jobService.GetJobAnalysis(jobId);

            var mappedAnalyzedPosts = analyzedPosts.Select(AnalyzedPostDto.FromModel).ToList();
            return Ok(mappedAnalyzedPosts);
        }

        private async Task<bool> IsAuthorizedToSeeJob(Guid jobId)
        {
            var job = await _storageService.GetJob(jobId);
            return job.Username == User.Identity.Name;
        }
    }
}

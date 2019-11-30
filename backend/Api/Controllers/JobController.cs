using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Models;
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
        [Route("api/job/{jobId:guid}/aggregation_analysis")]
        // TODO: change request model here (analysis type is not required here)
        public async Task<ActionResult<AggregationAnalysisResponse>> GetJobAnalysisAggregation([FromRoute]Guid jobId, [FromBody] GetAggregationAnalysisStorageRequest analysisRequest)
        {
            /* TODO: uncomment this
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            */

            var analysisResult = await _storageService.GetAnalysisAggregation(analysisRequest);
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/array_analysis")]
        // TODO: change request model here (analysis type is not required here)
        public async Task<ActionResult<ArrayAnalysisResponse>> GetJobAnalysisArray([FromRoute]Guid jobId, [FromBody] GetArrayAnalysisStorageRequest analysisRequest)
        {
            /* TODO: uncomment this
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            */

            var analysisResult = await _storageService.GetAnalysisArray(analysisRequest);
            var analysisResponse = ArrayAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }

        private async Task<bool> IsAuthorizedToSeeJob(Guid jobId)
        {
            var job = await _storageService.GetJob(jobId);
            return job.Username == User.Identity.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Services;
using IAuthorizationService = Socneto.Domain.Services.IAuthorizationService;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IJobService _jobService;
        private readonly IGetAnalysisService _getAnalysisService;
        private readonly ILogger<JobController> _logger;
        
        public JobController(IAuthorizationService authorizationService, IJobService jobService, 
            IGetAnalysisService getAnalysisService, ILogger<JobController> logger)
        {
            _authorizationService = authorizationService;
            _jobService = jobService;
            _getAnalysisService = getAnalysisService;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("api/job/{username}/all")]
        public async Task<ActionResult<List<JobDto>>> GetJobs([FromRoute] string username)
        {
            if (!IsAuthenticatedTo(username))
                return Unauthorized();
            
            var jobStatuses = await _jobService.GetJobsDetails(username);

            var mappedJobStatuses = jobStatuses
                .Select(JobDto.FromModel)
                .ToList();
            
            return Ok(mappedJobStatuses);
        }

        [HttpPost]
        [Route("api/job/{username}/create")]
        public async Task<ActionResult<JobDto>> SubmitJob([FromRoute] string username, [FromBody] JobSubmitRequest request)
        {
            if (!IsAuthenticatedTo(username))
                return Unauthorized();
            
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/status")]
        public async Task<ActionResult<JobDto>> GetJobDetail([FromRoute] Guid jobId)
        {
            if (! await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
                return Unauthorized();
            
            var jobDetail = await _jobService.GetJobDetail(jobId);

            var jobStatusResponse = JobDto.FromModel(jobDetail);
            return Ok(jobStatusResponse);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/posts")]
        public async Task<ActionResult<Paginated<AnalyzedPostDto>>> GetJobPosts([FromRoute] Guid jobId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (! await  _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
                return Unauthorized();

            var offset = (page - 1) * size;
            var (posts, postsCount) = await _jobService.GetJobPosts(jobId, offset, size);

            var postDtos = posts.Select(AnalyzedPostDto.FromModel).ToList();
            var paginatedPosts = new Paginated<AnalyzedPostDto>
            {
                Pagination = new Pagination
                {
                    TotalSize = postsCount,
                    Page = Math.Clamp(page, 1, ((postsCount - 1) / size) + 1),
                    PageSize = size
                },
                Data = postDtos
            };
            
            return Ok(paginatedPosts);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/aggregation_analysis")]
        public async Task<ActionResult<AggregationAnalysisResponse>> GetJobAnalysisAggregation([FromRoute]Guid jobId, [FromBody] GetAggregationAnalysisRequest analysisRequest)
        {
            if (! await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
                return Unauthorized();

            var analysisResult = await _getAnalysisService.GetAggregationAnalysis(analysisRequest.AnalyserId, analysisRequest.AnalysisProperty);
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/array_analysis")]
        public async Task<ActionResult<ArrayAnalysisResponse>> GetJobAnalysisArray([FromRoute]Guid jobId, [FromBody] GetArrayAnalysisRequest analysisRequest)
        {
            if (! await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
                return Unauthorized();
            
            var analysisResult = await _getAnalysisService.GetArrayAnalysis(analysisRequest.AnalyserId, analysisRequest.AnalysisProperties, analysisRequest.IsXPostDate);
            var analysisResponse = ArrayAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        private bool IsAuthenticatedTo(string username)
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            
            return username == User.Identity.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Socneto.Api.Models;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
using Socneto.Domain.Services;
using IAuthorizationService = Socneto.Domain.Services.IAuthorizationService;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobController : SocnetoController
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IJobService _jobService;
        private readonly IJobManagementService _jobManagementService;
        private readonly IGetAnalysisService _getAnalysisService;
        private readonly IEventTracker<JobController> _eventTracker;
        
        public JobController(IAuthorizationService authorizationService, IJobService jobService, 
            IJobManagementService jobManagementService, IGetAnalysisService getAnalysisService, 
            IEventTracker<JobController> eventTracker)
        {   
            _authorizationService = authorizationService;
            _jobService = jobService;
            _jobManagementService = jobManagementService;
            _getAnalysisService = getAnalysisService;
            _eventTracker = eventTracker;
        }

        [HttpGet]
        [Route("api/job/all")]
        public async Task<ActionResult<List<JobDto>>> GetJobs()
        {
            var username = User.Identity.Name;
            if (username == null)
            {
                _eventTracker.TrackInfo("GetJobs", "Unauthorized attempt to list all jobs");
                return Unauthorized();
            }
            
            var jobStatuses = await _jobService.GetJobsDetails(username);

            var mappedJobStatuses = jobStatuses
                .Select(JobDto.FromModel)
                .ToList();
            
            return Ok(mappedJobStatuses);
        }

        [HttpPost]
        [Route("api/job/create")]
        public async Task<ActionResult<JobStatusResponse>> SubmitJob([FromBody] JobSubmitRequest request)
        {
            _eventTracker.TrackInfo("SubmitJob", "Submitting a new job", JsonConvert.SerializeObject(request));
            if (User.Identity.Name == null)
            {
                _eventTracker.TrackInfo("SubmitJob", "Unauthorized attempt to list all jobs");
                return Unauthorized();
            }
            
            if (request.Credentials == null)
            {
                request.Credentials = new Dictionary<string, Dictionary<string, string>>();
            }

            var jobSubmit = new JobSubmit
            {
                JobName = request.JobName,
                TopicQuery = request.TopicQuery,
                SelectedAcquirersIdentifiers = request.SelectedAcquirers,
                SelectedAnalysersIdentifiers = request.SelectedAnalysers,
                Language = request.Language,
            };

            var jobStatus = await _jobManagementService.SubmitJob(jobSubmit, request.Credentials);
            var response = new JobStatusResponse
            {
                JobId = jobStatus.JobId,
                Status = jobStatus.Status,
            };
            _eventTracker.TrackInfo("SubmitJob", "New job submitted", JsonConvert.SerializeObject(response));
            return Ok(response);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/stop")]
        public async Task<ActionResult<JobStatusResponse>> StopJob([FromRoute] Guid jobId)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("StopJob", $"User '{User.Identity.Name}' is not authorized to stop job '{jobId}'");
                return Unauthorized();
            }

            var jobStatus = await _jobManagementService.StopJob(jobId);
            var response = new JobStatusResponse
            {
                JobId = jobStatus.JobId,
                Status = jobStatus.Status,
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/status")]
        public async Task<ActionResult<JobDto>> GetJobDetail([FromRoute] Guid jobId)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobDetail", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            var jobDetail = await _jobService.GetJobDetail(jobId);

            var jobStatusResponse = JobDto.FromModel(jobDetail);
            return Ok(jobStatusResponse);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/posts")]
        public async Task<ActionResult<Paginated<AnalyzedPostDto>>> GetJobPosts([FromRoute] Guid jobId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobPosts", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

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
            _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' requested aggregation analysis", 
                JsonConvert.SerializeObject(analysisRequest));
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            AggregationAnalysisResult analysisResult;
            try
            {
                analysisResult = await _getAnalysisService.GetAggregationAnalysis(jobId, analysisRequest.AnalyserId,
                    analysisRequest.AnalysisProperty);
            }
            catch (GetAnalysisService.GetAnalysisException)
            {
                return AggregationAnalysisResponse.Empty();
            }

            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/array_analysis")]
        public async Task<ActionResult<ArrayAnalysisResponse>> GetJobAnalysisArray([FromRoute]Guid jobId, [FromBody] GetArrayAnalysisRequest analysisRequest)
        {
            _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' requested array analysis", 
                JsonConvert.SerializeObject(analysisRequest));
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            ArrayAnalysisResult analysisResult;
            try
            {
                analysisResult = await _getAnalysisService.GetArrayAnalysis(jobId, analysisRequest.AnalyserId,
                    analysisRequest.AnalysisProperties, analysisRequest.IsXPostDate);
            }
            catch (GetAnalysisService.GetAnalysisException)
            {
                return ArrayAnalysisResponse.Empty();
            }

            var analysisResponse = ArrayAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
    }
}

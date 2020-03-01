using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Socneto.Api.Models;
using Socneto.Api.Models.Requests;
using Socneto.Api.Models.Responses;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;
using Socneto.Domain.Services;
using IAuthorizationService = Socneto.Domain.Services.IAuthorizationService;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobController : SocnetoController
    {
        private const int ExportPageSize = 200;
        
        private readonly IAuthorizationService _authorizationService;
        private readonly IJobService _jobService;
        private readonly IJobManagementService _jobManagementService;
        private readonly IGetAnalysisService _getAnalysisService;
        private readonly ICsvService _csvService;
        private readonly IEventTracker<JobController> _eventTracker;
        
        public JobController(IAuthorizationService authorizationService, IJobService jobService, 
            IJobManagementService jobManagementService, IGetAnalysisService getAnalysisService, 
            ICsvService csvService, IEventTracker<JobController> eventTracker)
        {   
            _authorizationService = authorizationService;
            _jobService = jobService;
            _jobManagementService = jobManagementService;
            _getAnalysisService = getAnalysisService;
            _csvService = csvService;
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
            _eventTracker.TrackInfo("SubmitJob", "Submitting a new job", request);
            if (User.Identity.Name == null)
            {
                _eventTracker.TrackInfo("SubmitJob", "Unauthorized attempt to list all jobs");
                return Unauthorized();
            }
            
            if (request.Attributes == null)
            {
                request.Attributes = new Dictionary<string, Dictionary<string, string>>();
            }

            var jobSubmit = new JobSubmit
            {
                JobName = request.JobName,
                TopicQuery = request.TopicQuery,
                SelectedAcquirersIdentifiers = request.SelectedAcquirers,
                SelectedAnalysersIdentifiers = request.SelectedAnalysers,
            };

            var jobStatus = await _jobManagementService.SubmitJob(jobSubmit, request.Attributes);
            var response = new JobStatusResponse
            {
                JobId = jobStatus.JobId,
                Status = jobStatus.Status,
            };
            _eventTracker.TrackInfo("SubmitJob", "New job submitted", response);
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
        public async Task<ActionResult<Paginated<AnalyzedPostDto>>> GetJobPosts([FromRoute] Guid jobId, 
            [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 20, 
            [FromQuery(Name = "contains_words")] string[] containsWords = null,
            [FromQuery(Name = "exclude_words")] string[] excludeWords = null,
            [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            containsWords = containsWords ?? new string[0];
            excludeWords = excludeWords ?? new string[0];
            page = Math.Max(1, page);
            
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobPosts", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            var (posts, postsCount) = await _jobService.GetJobPosts(jobId, containsWords, excludeWords, from, to, page, pageSize);
            var maxPage = Math.Max(1, (postsCount - 1) / pageSize + 1);
            if (page > maxPage)
            {
                page = maxPage;
                (posts, postsCount) = await _jobService.GetJobPosts(jobId, containsWords, excludeWords, from, to, page, pageSize);
            }

            var postDtos = posts.Select(PostDto.FromModel).ToList();
            var paginatedPosts = new Paginated<PostDto>
            {
                Pagination = new Pagination
                {
                    TotalSize = postsCount,
                    Page = page,
                    PageSize = pageSize
                },
                Data = postDtos
            };
            
            return Ok(paginatedPosts);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/job/{jobId:guid}/posts/export")]
        [Produces("text/csv")]
        public async Task<IActionResult> JobPostsExport([FromRoute] Guid jobId,
            [FromQuery(Name = "contains_words")] string[] containsWords = null,
            [FromQuery(Name = "exclude_words")] string[] excludeWords = null,
            [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            containsWords = containsWords ?? new string[0];
            excludeWords = excludeWords ?? new string[0];
            
            var currentPage = 1;
            var (currentPagePosts, postsCount) = await _jobService.GetJobPosts(jobId, containsWords, excludeWords, from, to,currentPage, ExportPageSize);
            var csvStringBuilder = new StringBuilder();
            csvStringBuilder.Append(_csvService.GetCsv(currentPagePosts.Select(PostDto.FromModel).ToList(), true));
            
            while (currentPage < (postsCount / ExportPageSize) + 1)
            {
                currentPage++;
                (currentPagePosts, postsCount) = await _jobService.GetJobPosts(jobId, containsWords, excludeWords, from, to, currentPage, ExportPageSize);
                csvStringBuilder.Append(_csvService.GetCsv(currentPagePosts.Select(PostDto.FromModel).ToList(), false));
            }
            
            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(csvStringBuilder.ToString()));
            return new FileStreamResult(dataStream, "text/csv") {FileDownloadName = $"socneto_export_{jobId}.csv"};
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/posts_frequency")]
        public async Task<ActionResult<AggregationAnalysisResponse>> GetPostsFrequency([FromRoute] Guid jobId)
        {
            _eventTracker.TrackInfo("GetPostsFrequency", $"User '{User.Identity.Name}' requested posts frequency for job '{jobId}'");
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetPostsFrequency", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            var analysisResult = await _getAnalysisService.GetPostsFrequency(jobId);
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
        
            return Ok(analysisResponse);
        }
        
        [HttpGet]
        [Route("api/job/{jobId:guid}/language_frequency")]
        public async Task<ActionResult<AggregationAnalysisResponse>> GetLanguageFrequency([FromRoute] Guid jobId)
        {
            _eventTracker.TrackInfo("GetLanguageFrequency", $"User '{User.Identity.Name}' requested language frequency for job '{jobId}'");
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetLanguageFrequency", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            var analysisResult = await _getAnalysisService.GetLanguageFrequency(jobId);
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
        
            return Ok(analysisResponse);
        }
        
        [HttpGet]
        [Route("api/job/{jobId:guid}/author_frequency")]
        public async Task<ActionResult<AggregationAnalysisResponse>> GetAuthorFrequency([FromRoute] Guid jobId)
        {
            _eventTracker.TrackInfo("GetAuthorFrequency", $"User '{User.Identity.Name}' requested author frequency for job '{jobId}'");
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetAuthorFrequency", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            var analysisResult = await _getAnalysisService.GetAuthorFrequency(jobId);
            analysisResult.MapResult.Remove("0"); // Artificial author id for posts, where no author is defined
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
        
            return Ok(analysisResponse);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/aggregation_analysis")]
        public async Task<ActionResult<AggregationAnalysisResponse>> GetJobAnalysisAggregation([FromRoute]Guid jobId, [FromBody] GetAggregationAnalysisRequest analysisRequest)
        {
            _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' requested aggregation analysis", analysisRequest);
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobAnalysisAggregation", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            try
            {
                var analysisResult = await _getAnalysisService.GetAggregationAnalysis(jobId, analysisRequest.AnalyserId,
                    analysisRequest.AnalysisProperty);
                
                var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
            
                return Ok(analysisResponse);
            }
            catch (GetAnalysisService.GetAnalysisException e)
            {
                _eventTracker.TrackError("GetAggregationAnalysis", e.Message, analysisRequest);
                return Ok(AggregationAnalysisResponse.Empty());
            }
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/array_analysis")]
        public async Task<ActionResult<ArrayAnalysisResponse>> GetJobAnalysisArray([FromRoute]Guid jobId, [FromBody] GetArrayAnalysisRequest analysisRequest)
        {
            _eventTracker.TrackInfo("GetJobAnalysisArray", $"User '{User.Identity.Name}' requested array analysis", 
                JsonConvert.SerializeObject(analysisRequest));
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobAnalysisArray", $"User '{User.Identity.Name}' is not authorized to see job '{jobId}'");
                return Unauthorized();
            }

            ArrayAnalysisResult analysisResult;
            try
            {
                analysisResult = await _getAnalysisService.GetArrayAnalysis(jobId, analysisRequest.AnalyserId,
                    analysisRequest.AnalysisProperties, analysisRequest.IsXPostDate, analysisRequest.PageSize, analysisRequest.Page - 1);
            }
            catch (GetAnalysisService.GetAnalysisException e)
            {
                _eventTracker.TrackError("GetArrayAnalysis", e.Message, analysisRequest);
                return Ok(ArrayAnalysisResponse.Empty());
            }

            var analysisResponse = ArrayAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
    }
}

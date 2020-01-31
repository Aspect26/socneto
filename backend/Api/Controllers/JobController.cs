using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Socneto.Api.Models;
using Socneto.Domain;
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
        private readonly DefaultAcquirersCredentialsOptions _defaultAcquirersCredentials;
        private readonly ILogger<JobController> _logger;
        
        public JobController(IAuthorizationService authorizationService, IJobService jobService, 
            IJobManagementService jobManagementService, IGetAnalysisService getAnalysisService, 
            IOptions<DefaultAcquirersCredentialsOptions> defaultAcquirersCredentialsOptionsObject, 
            ILogger<JobController> logger)
        {
            if (defaultAcquirersCredentialsOptionsObject.Value?.Twitter == null)
                throw new ArgumentNullException(nameof(defaultAcquirersCredentialsOptionsObject.Value.Twitter));
            
            if (defaultAcquirersCredentialsOptionsObject.Value?.Reddit == null)
                throw new ArgumentNullException(nameof(defaultAcquirersCredentialsOptionsObject.Value.Reddit));
            
            _authorizationService = authorizationService;
            _jobService = jobService;
            _jobManagementService = jobManagementService;
            _getAnalysisService = getAnalysisService;
            _defaultAcquirersCredentials = defaultAcquirersCredentialsOptionsObject.Value;
            _logger = logger;
        }

        private AcquirerCredentials DefaultCredentials => new AcquirerCredentials
        {
            TwitterCredentials = DefaultTwitterCredentials,
            RedditCredentials = DefaultRedditCredentials
        };

        private TwitterCredentials DefaultTwitterCredentials => new TwitterCredentials
        {
            ApiKey = _defaultAcquirersCredentials.Twitter.ApiKey,
            ApiSecretKey = _defaultAcquirersCredentials.Twitter.ApiSecretKey,
            AccessToken = _defaultAcquirersCredentials.Twitter.AccessToken,
            AccessTokenSecret = _defaultAcquirersCredentials.Twitter.AccessTokenSecret,
        };

        private RedditCredentials DefaultRedditCredentials => new RedditCredentials
        {
            AppId = _defaultAcquirersCredentials.Reddit.AppId,
            AppSecret = _defaultAcquirersCredentials.Reddit.AppSecret,
            RefreshToken = _defaultAcquirersCredentials.Reddit.RefreshToken
        };

        [HttpGet]
        [Route("api/job/all")]
        public async Task<ActionResult<List<JobDto>>> GetJobs()
        {
            var username = User.Identity.Name;
            if (username == null)
            {
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
            if (User.Identity.Name == null)
            {
                return Unauthorized();
            }
            
            if (request.Credentials == null)
            {
                request.Credentials = new Dictionary<string, AcquirerCredentials>();
            }

            var attributes = new JObject();
            foreach (var selectedAcquirer in request.SelectedAcquirers)
            {
                var acquirerCredentials = request.Credentials.ContainsKey(selectedAcquirer)
                    ? request.Credentials[selectedAcquirer]
                    : DefaultCredentials;
                var twitterCredentials = acquirerCredentials.TwitterCredentials ?? DefaultTwitterCredentials;
                var redditCredentials = acquirerCredentials.RedditCredentials ?? DefaultRedditCredentials;
                attributes.Add(selectedAcquirer, new JObject(
                    new JProperty("ApiKey", twitterCredentials.ApiKey),
                    new JProperty("ApiSecretKey", twitterCredentials.ApiSecretKey),    
                    new JProperty("AccessToken", twitterCredentials.AccessToken),
                    new JProperty("AccessTokenSecret", twitterCredentials.AccessTokenSecret),
                    
                    new JProperty("RedditAppId", redditCredentials.AppId),
                    new JProperty("RedditAppSecret", redditCredentials.AppSecret),
                    new JProperty("RedditRefreshToken", redditCredentials.RefreshToken)
                ));
            }
            
            var jobSubmit = new JobSubmit
            {
                JobName = request.JobName,
                TopicQuery = request.TopicQuery,
                SelectedAcquirersIdentifiers = request.SelectedAcquirers,
                SelectedAnalysersIdentifiers = request.SelectedAnalysers,
                Language = request.Language,
                Attributes = attributes
            };

            var jobStatus = await _jobManagementService.SubmitJob(jobSubmit);
            var response = new JobStatusResponse
            {
                JobId = jobStatus.JobId,
                Status = jobStatus.Status,
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/stop")]
        public async Task<ActionResult<JobStatusResponse>> StopJob([FromRoute] Guid jobId)
        {
            if (! await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
                return Unauthorized();

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
    }
}

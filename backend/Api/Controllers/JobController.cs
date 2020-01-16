﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IGetAnalysisService _getAnalysisService;
        private readonly ILogger<JobController> _logger;
        
        public JobController(IJobService jobService, IGetAnalysisService getAnalysisService, 
            ILogger<JobController> logger)
        {
            _jobService = jobService;
            _getAnalysisService = getAnalysisService;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("api/job/{username}/all")]
        public async Task<ActionResult<List<JobDto>>> GetJobs([FromRoute]string username)
        {
            if (!IsAuthorizedToSeeUser(username))
                return Unauthorized();
            
            var jobStatuses = await _jobService.GetJobsDetails(username);

            var mappedJobStatuses = jobStatuses
                .Select(JobDto.FromModel)
                .ToList();
            
            return Ok(mappedJobStatuses);
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
        public async Task<ActionResult<Paginated<AnalyzedPostDto>>> GetJobPosts([FromRoute] Guid jobId, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (! await  IsAuthorizedToSeeJob(jobId))
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
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var analysisResult = await _getAnalysisService.GetAggregationAnalysis(analysisRequest.AnalyserId, analysisRequest.AnalysisProperty);
            var analysisResponse = AggregationAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        [HttpPost]
        [Route("api/job/{jobId:guid}/array_analysis")]
        public async Task<ActionResult<ArrayAnalysisResponse>> GetJobAnalysisArray([FromRoute]Guid jobId, [FromBody] GetArrayAnalysisRequest analysisRequest)
        {
            if (! await  IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var analysisResult = await _getAnalysisService.GetArrayAnalysis(analysisRequest.AnalyserId, analysisRequest.AnalysisProperties);
            var analysisResponse = ArrayAnalysisResponse.FromModel(analysisResult);
            
            return Ok(analysisResponse);
        }
        
        private bool IsAuthorizedToSeeUser(string username)
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            
            return username == User.Identity.Name;
        }

        private async Task<bool> IsAuthorizedToSeeJob(Guid jobId)
        {
            /*  TODO: uncomment this
            var job = await _storageService.GetJob(jobId);
            return job.Username == User.Identity.Name;
            */
            return await Task.FromResult(true);
        }
    }
}

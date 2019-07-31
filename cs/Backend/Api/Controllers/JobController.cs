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
        private readonly IJobResultService _jobResultService;


        public JobController(IJobResultService jobResultService)
        {
            _jobResultService = jobResultService;
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/status")]
        public async Task<ActionResult<JobStatusResponse>> GetJobStatus([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var jobStatus = await _jobResultService.GetJobStatus(jobId);

            var jobStatusResponse = JobStatusResponse.FromModel(jobStatus);
            return Ok(jobStatusResponse);
        }
        
        [HttpGet]
        [Route("api/job/{jobId:guid}/analysis")]
        public async Task<ActionResult<List<AnalyzedPostDto>>> GetJobAnalysis([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var analyzedPosts = await _jobResultService.GetJobAnalysis(jobId);

            var mappedAnalyzedPosts = analyzedPosts.Select(AnalyzedPostDto.FromModel).ToList();
            return Ok(mappedAnalyzedPosts);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/job/{jobId:guid}/result")]
        public async Task<ActionResult<JobResultResponse>> GetJobResult([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();
            
            var jobResult = await _jobResultService.GetJobResult(jobId);

            var jobResultResponse = JobResultResponse.FromModel(jobResult);
            return Ok(jobResultResponse);
        }
        
        private bool IsAuthorizedToSeeJob(Guid jobId)
        {
            // TODO: check if the job belongs to the authorized user
            return true;
        }
    }
}

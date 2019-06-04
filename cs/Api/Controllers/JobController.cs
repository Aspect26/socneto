﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain;
using Socneto.Domain.QueryResult;

namespace Socneto.Api.Controllers
{
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IQueryJobResultService _queryJobResultService;


        public JobController(IJobService jobService, IQueryJobResultService queryJobResultService)
        {
            _jobService = jobService;
            _queryJobResultService = queryJobResultService;
        }
        
        [HttpGet]
        [Route("api/job/{jobId:guid}/status")]
        public async Task<ActionResult<JobStatusResponse>> GetJobStatus([FromRoute]Guid jobId)
        {
            var jobStatus = await _queryJobResultService.GetJobStatus(jobId);

            var jobStatusResponse = JobStatusResponse.FromModel(jobStatus);
            return Ok(jobStatusResponse);
        }



        [HttpGet]
        [Route("api/job/{jobId:guid}/result")]
        public async Task<ActionResult<JobResultResponse>> GetJobResult([FromRoute]Guid jobId)
        {
            var jobResult = await _queryJobResultService.GetJobResult(jobId);

            var jobResultResponse = JobResultResponse.FromModel(jobResult);
            return Ok(jobResultResponse);
        }


    }
}

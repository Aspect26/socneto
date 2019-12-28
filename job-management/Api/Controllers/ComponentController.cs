using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Domain.ComponentManagement;
using Domain.SubmittedJobConfiguration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [ApiController]
    public class ComponentController : ControllerBase
    {
        private readonly ISubscribedComponentManager _subscribedComponentManager;

        public ComponentController(ISubscribedComponentManager subscribedComponentManager)
        {
            _subscribedComponentManager = subscribedComponentManager;
        }

        [HttpPost]
        [Route("/api/job/submit")]
        public async Task<ActionResult<JobResponse>> SubmitJob()
        {
            var body = await new System.IO.StreamReader(this.Request.Body).ReadToEndAsync();
            JobSubmitRequest jobSubmitRequest = JsonConvert.DeserializeObject<JobSubmitRequest>(body);
            if (jobSubmitRequest == null)
            {
                return BadRequest("Invalid body");
            }
            if (jobSubmitRequest?.SelectedDataAnalysers?.Any() == false)
            {
                return BadRequest("No analysers were selected");
            }
            if (jobSubmitRequest?.SelectedDataAcquirers?.Any() == false)
            {
                return BadRequest("No analysers were selected");
            }

            var jobId = Guid.NewGuid();
            var jobConfigUpdateNotification = JobConfigUpdateCommand.NewJob(
                jobId,
                jobSubmitRequest.JobName,
                jobSubmitRequest.SelectedDataAnalysers,
                jobSubmitRequest.SelectedDataAcquirers,
                jobSubmitRequest.TopicQuery,
                jobSubmitRequest.Language,
                jobSubmitRequest.Attributes
                );

            var configUpdateResult = await _subscribedComponentManager
                .StartJobAsync(jobConfigUpdateNotification);

            if (configUpdateResult.HasError)
            {
                return BadRequest($"Job submit failed, error: {configUpdateResult.Error}");
            }

            var jobSubmitResponse = new JobResponse(
                configUpdateResult.JobId,
                configUpdateResult.Status);
            return Ok(jobSubmitResponse);
        }

        [HttpPost]
        [Route("/api/job/stop/{jobId}")]
        public async Task<ActionResult<JobResponse>> StopJob(
            [FromRoute]Guid jobId)
        {
            var result = await _subscribedComponentManager.StopJob(jobId);

            if (result.HasError)
            {
                return BadRequest($"Job submit failed, error: {result.Error}");
            }

            var jobSubmitResponse = new JobResponse(
                result.JobId,
                result.Status);
            return Ok(jobSubmitResponse);
        }
    }
}

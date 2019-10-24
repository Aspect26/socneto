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
        public async Task<ActionResult<JobSubmitResponse>> SubmitJob(
            [FromBody]JobSubmitRequest jobSubmitRequest)
        {
            var jobId = Guid.NewGuid();
            var jobConfigUpdateNotification = new JobConfigUpdateNotification(
                jobId,
                jobSubmitRequest.SelectedAnalysers,
                jobSubmitRequest.SelectedNetworks,
                jobSubmitRequest.TopicQuery);

            try
            {
                await _subscribedComponentManager.PushJobConfigUpdateAsync(jobConfigUpdateNotification);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            var jobSubmitResponse = new JobSubmitResponse();
            return Ok(jobSubmitResponse);
        }
    }
}

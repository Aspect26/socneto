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
    public class ComponentController: ControllerBase
    {
        private readonly ISubscribedComponentManager _subscribedComponentManager;

        public ComponentController(ISubscribedComponentManager subscribedComponentManager )
        {
            _subscribedComponentManager = subscribedComponentManager;
        }
        
        [HttpGet]
        [Route("/api/components/networks")]
        public ActionResult<ComponentListResponse> GetAvailableNetworkComponents()
        {
            var networkComponents = _subscribedComponentManager
                .GetAvaliableNetworks()
                .Select(r => 
                    new ComponentsResponse(
                        r.ComponentId, 
                        r.ComponentType))
                .ToList();
            var componentList = new ComponentListResponse(networkComponents);
            return Ok(componentList);
        }

        [HttpGet]
        [Route("/api/components/analysers")]
        public ActionResult<ComponentListResponse> GetAvailableAnalysersComponents()
        {
            var networkComponents = _subscribedComponentManager
                .GetAvaliableAnalysers()
                .Select(r =>
                    new ComponentsResponse(
                        r.ComponentId,
                        r.ComponentType))
                .ToList();
            var componentList = new ComponentListResponse(networkComponents);
            return Ok(componentList);
        }

        [HttpPost]
        [Route("/api/job/submit")]
        public async Task< ActionResult<JobSubmitResponse>> SubmitJob(
            [FromBody]JobSubmitRequest jobSubmitRequest
            )
        {

            var jobConfigUpdateNotification = new JobConfigUpdateNotification(
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

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
            //[FromBody]JobSubmitRequest jobSubmitRequest
            )
        {
            // TODO validation
            var jobSubmitRequest = new JobSubmitRequest
            {
                SelectedAnalysers = new List<string>(),
                TopicQuery = "SQ_1 and SQ_2",
                SelectedNetworks = new List<string>()
            };

            var jobConfigUpdateNotification = new JobConfigUpdateNotification(
                jobSubmitRequest.SelectedAnalysers,
                jobSubmitRequest.SelectedNetworks,
                jobSubmitRequest.TopicQuery);
                
            await _subscribedComponentManager.PushJobConfigUpdateAsync(jobConfigUpdateNotification);

            var jobSubmitReponse = new JobSubmitResponse();
            return Ok(jobSubmitReponse);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ChartsController : SocnetoController
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IChartsService _chartsService;
        private readonly IEventTracker<ComponentsController> _eventTracker;

        public ChartsController(IAuthorizationService authorizationService, IChartsService chartsService, 
            IEventTracker<ComponentsController> eventTracker)
        {
            _authorizationService = authorizationService;
            _chartsService = chartsService;
            _eventTracker = eventTracker;
        }

        [HttpGet]
        [Route("api/charts/{jobId:guid}")]
        public async Task<ActionResult<List<ChartDefinitionDto>>> GetJobCharts([FromRoute]Guid jobId)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("GetJobCharts", $"User '{User.Identity.Name}' is unauthorized to see job '{jobId}'");
                return Unauthorized();
            }

            var jobCharts = await _chartsService.GetJobCharts(jobId);
            var mappedChartDefinitions = jobCharts
                .Select(ChartDefinitionDto.FromModel)
                .ToList();
            
            return Ok(mappedChartDefinitions);
        }

        [HttpPost]
        [Route("api/charts/{jobId:guid}/create")]
        public async Task<ActionResult<ChartDefinitionDto>> CreateJobChart([FromRoute] Guid jobId, [FromBody] CreateChartDefinitionRequest request)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("CreateJobChart", $"User '{User.Identity.Name}' is unauthorized to see job '{jobId}'");
                return Unauthorized();
            }

            var analysisDataPaths = request.AnalysisDataPaths.Select(analysisProperty => new AnalysisDataPath
            {
                AnalyserComponentId = analysisProperty.AnalyserComponentId,
                Property = analysisProperty.Property
            }).ToList();
            
            var newChart = await _chartsService.CreateJobChart(jobId, request.Title, request.ChartType, analysisDataPaths, request.IsXPostDateTime);
            return Ok(ChartDefinitionDto.FromModel(newChart));
        }

        [HttpGet]
        [Route("api/charts/{jobId:guid}/{chartId:guid}/remove")]
        public async Task<ActionResult<List<ChartDefinitionDto>>> RemoveJobChart([FromRoute] Guid jobId, [FromRoute] Guid chartId)
        {
            if (!await _authorizationService.IsUserAuthorizedToSeeJob(User.Identity.Name, jobId))
            {
                _eventTracker.TrackInfo("RemoveJobChart", $"User '{User.Identity.Name}' is unauthorized to see job '{jobId}'");
                return Unauthorized();
            }
            
            var newJobView = await _chartsService.RemoveJobChart(jobId, chartId);
            if (newJobView == null)
            {
                return NotFound();
            }

            var result = newJobView.ViewConfiguration.ChartDefinitions
                .Select(ChartDefinitionDto.FromModel)
                .ToList();
                
            return Ok(result);
        }
    }
}
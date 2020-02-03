using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
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
        public async Task<ActionResult<SuccessResponse>> CreateJobChart([FromRoute] Guid jobId, [FromBody] CreateChartDefinitionRequest request)
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
            
            await _chartsService.CreateJobChart(jobId, request.ChartType, analysisDataPaths, request.IsXPostDateTime);
            return Ok(SuccessResponse.True());
        }
    }
}
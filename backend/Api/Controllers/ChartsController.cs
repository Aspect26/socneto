using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Models;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        // TODO: maybe create charts service
        private readonly IStorageService _storageService;
        private ILogger<ComponentsController> _logger;

        public ChartsController(IStorageService storageService, ILogger<ComponentsController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/charts/{jobId:guid}")]
        public async Task<ActionResult<List<ChartDefinitionDto>>> GetJobCharts([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var jobView = await _storageService.GetJobView(jobId);

            if (jobView?.ViewConfiguration == null)
            {
                return Ok(new ArrayList());
            }

            var mappedChartDefinitions = jobView.ViewConfiguration.ChartDefinitions
                .Select(ChartDefinitionDto.FromModel)
                .ToList();
            
            return Ok(mappedChartDefinitions);
        }

        [HttpPost]
        [Route("api/charts/{jobId:guid}/create")]
        public async Task<ActionResult<SuccessResponse>> CreateJobChart([FromRoute] Guid jobId, [FromBody] CreateChartDefinitionRequest request)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var newChartDefinition = new ChartDefinition
            {
                ChartType = request.ChartType,
                AnalysisDataPaths = request.AnalysisDataPaths.Select(x => new AnalysisDataPath
                {
                    AnalyserComponentId = x.AnalyserComponentId,
                    Property = new AnalysisProperty
                    {
                        Identifier = x.Property.Identifier,
                        Type = x.Property.Type
                    }
                }).ToList(),
                IsXPostDatetime = request.IsXPostDateTime
            };

            var jobView = await _storageService.GetJobView(jobId);

            if (jobView.ViewConfiguration == null)
            {
                jobView.ViewConfiguration = new ViewConfiguration {ChartDefinitions = new List<ChartDefinition>() };
            }

            if (jobView.ViewConfiguration.ChartDefinitions == null)
            {
                jobView.ViewConfiguration.ChartDefinitions = new List<ChartDefinition>();
            }
            
            jobView.ViewConfiguration.ChartDefinitions.Add(newChartDefinition);

            await _storageService.UpdateJobView(jobId, jobView);
            
            return Ok(SuccessResponse.True());
        }
        
        private bool IsAuthorizedToSeeJob(Guid jobId)
        {
            // TODO: check if the job belongs to the authorized user
            return true;
        }
    }
}
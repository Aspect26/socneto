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
        
        private IStorageService _storageService;
        private ILogger<ComponentsController> _logger;

        public ChartsController(IStorageService storageService, ILogger<ComponentsController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/job/{jobId:guid}/charts")]
        public async Task<ActionResult<List<ChartDefinitionDto>>> GetJobCharts([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            var jobView = await _storageService.GetJobView(jobId);

            if (jobView == null)
            {
                return Ok(new ArrayList());
            }

            var mappedChartDefinitions = jobView.ViewConfiguration
                .Select(ChartDefinitionDto.FromModel)
                .ToList();
            
            return Ok(mappedChartDefinitions);
        }

        [HttpPost]
        [Route("api/job/{jobId:guid}/charts/create")]
        public async Task<ActionResult<SuccessResponse>> CreateJobChart([FromRoute] Guid jobId, [FromBody] CreateChartDefinitionRequest request)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            if (!Enum.TryParse(request.ChartType, out ChartType chartType))
            {
                return BadRequest();
            }

            var newChartDefinition = new ChartDefinition
            {
                ChartType = chartType,
                AnalysisDataPaths = request.AnalysisDataPaths
            };

            var jobView = await _storageService.GetJobView(jobId);
            if (jobView == null)
            {
                var viewConfiguration = new List<ChartDefinition> { newChartDefinition };

                jobView =  new JobView
                {
                    JobId = jobId,
                    ViewConfiguration = viewConfiguration
                };

                await _storageService.StoreJobView(jobId, jobView);
            }
            else
            {
                jobView.ViewConfiguration.Add(newChartDefinition);
            }
            
            return Ok(SuccessResponse.True());
        }
        
        private bool IsAuthorizedToSeeJob(Guid jobId)
        {
            // TODO: check if the job belongs to the authorized user
            return true;
        }
    }
}
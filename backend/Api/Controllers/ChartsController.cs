using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socneto.Api.Models;
using Socneto.Domain.Models;

namespace Socneto.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class ChartsController : ControllerBase
    {

        [HttpGet]
        [Route("api/job/{jobId:guid}/charts")]
        public async Task<ActionResult<List<ChartDefinitionDto>>> GetJobCharts([FromRoute]Guid jobId)
        {
            if (!IsAuthorizedToSeeJob(jobId))
                return Unauthorized();

            // TODO: get from DB
            var chartDefinitions = new List<ChartDefinition>();
            chartDefinitions.Add(new ChartDefinition{ChartType = ChartType.Line, JsonDataPaths = new List<string>() { "DataAnalyzer_Mock.polarity" }});
            chartDefinitions.Add(new ChartDefinition{ChartType = ChartType.Line, JsonDataPaths = new List<string>() { "DataAnalyzer_Mock.polarity", "DataAnalyzer_Mock.accuracy" }});
            
            var mappedChartDefinitions = chartDefinitions
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

            var chartDefinition = new ChartDefinition
            {
                JsonDataPaths = request.JsonDataPaths,
                ChartType = chartType
            };
            
            // TODO: store to DB
            return Ok(SuccessResponse.True());
        }
        
        private bool IsAuthorizedToSeeJob(Guid jobId)
        {
            // TODO: check if the job belongs to the authorized user
            return true;
        }
    }
}
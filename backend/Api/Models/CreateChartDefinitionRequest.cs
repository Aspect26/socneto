using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class CreateChartDefinitionRequest
    {
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        public string ChartType { get; set; }
    }
}
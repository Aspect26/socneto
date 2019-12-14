using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ChartDefinitionDto
    {
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        
        public string ChartType { get; set; }

        public static ChartDefinitionDto FromModel(ChartDefinition chartDefinition)
        {
            return new ChartDefinitionDto
            {
                AnalysisDataPaths = chartDefinition.AnalysisDataPaths,
                ChartType = chartDefinition.ChartType.ToString()
            };
        }
    }
}
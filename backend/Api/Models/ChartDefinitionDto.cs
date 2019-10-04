using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ChartDefinitionDto
    {
        public List<string> JsonDataPaths { get; set; }
        
        public string ChartType { get; set; }

        public static ChartDefinitionDto FromModel(ChartDefinition chartDefinition)
        {
            return new ChartDefinitionDto
            {
                JsonDataPaths = chartDefinition.JsonDataPaths,
                ChartType = chartDefinition.ChartType.ToString()
            };
        }
    }
}
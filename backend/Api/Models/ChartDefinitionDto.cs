using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ChartDefinitionDto
    {
        public string DataJsonPath { get; set; }
        
        public string ChartType { get; set; }

        public static ChartDefinitionDto FromModel(ChartDefinition chartDefinition)
        {
            return new ChartDefinitionDto
            {
                DataJsonPath = chartDefinition.DataJsonPath,
                ChartType = chartDefinition.ChartType.ToString()
            };
        }
    }
}
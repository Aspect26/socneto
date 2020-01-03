using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class ChartDefinitionDto
    {
        [JsonProperty("analysis_data_paths")]
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        
        [JsonProperty("chart_type")]
        public string ChartType { get; set; }
        
        [JsonProperty("is_x_datetime")]
        public bool IsXDateTime { get; set; }

        public static ChartDefinitionDto FromModel(ChartDefinition chartDefinition)
        {
            return new ChartDefinitionDto
            {
                AnalysisDataPaths = chartDefinition.AnalysisDataPaths,
                ChartType = chartDefinition.ChartType.ToString(),
                IsXDateTime = chartDefinition.IsXPostDatetime
            };
        }
    }
}
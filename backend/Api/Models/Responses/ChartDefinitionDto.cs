using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Api.Models.Responses
{
    public class ChartDefinitionDto
    {
        [JsonProperty("id")]
        public Guid Identifier { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
    
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
                Identifier = chartDefinition.Identifier,
                Title = chartDefinition.Title,
                AnalysisDataPaths = chartDefinition.AnalysisDataPaths,
                ChartType = chartDefinition.ChartType.ToString(),
                IsXDateTime = chartDefinition.IsXPostDatetime
            };
        }
    }
}
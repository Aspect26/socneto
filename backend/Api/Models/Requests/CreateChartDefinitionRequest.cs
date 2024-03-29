﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Api.Models.Requests
{
    public class CreateChartDefinitionRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    
        [JsonProperty("analysis_data_paths")]
        public List<AnalysisDataPathRequest> AnalysisDataPaths { get; set; }
        
        [JsonProperty("chart_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChartType ChartType { get; set; }
        
        [JsonProperty("is_x_post_datetime")]
        public bool IsXPostDateTime { get; set; }
    }

    public class AnalysisDataPathRequest
    {
        [JsonProperty("analyser_component_id")]
        public string AnalyserComponentId { get; set; }

        [JsonProperty("analyser_property")]
        public string Property { get; set; }
    }
}
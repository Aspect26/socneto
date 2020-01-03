using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Socneto.Domain.Models;


namespace Socneto.Api.Models
{
    public class CreateChartDefinitionRequest
    {
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
        public AnalysisPropertyRequest Property { get; set; }
    }

    public class AnalysisPropertyRequest
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Socneto.Domain.Models.AnalysisPropertyType Type { get; set; }
    }
}
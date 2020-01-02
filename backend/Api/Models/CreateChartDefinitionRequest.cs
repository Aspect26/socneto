using System.Collections.Generic;
using Newtonsoft.Json;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class CreateChartDefinitionRequest
    {
        [JsonProperty("analysis_data_paths")]
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        
        [JsonProperty("chart_type")]
        public string ChartType { get; set; }
        
        [JsonProperty("is_x_post_datetime")]
        public bool IsXPostDateTime { get; set; }
    }
}
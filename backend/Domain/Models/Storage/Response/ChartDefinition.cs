using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models.Storage.Response
{
    public class ChartDefinition
    {
        [JsonProperty("id")]
        public Guid Identifier { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("analysis_data_paths")]
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("chart_type")]
        public ChartType ChartType { get; set; }
        
        [JsonProperty("is_x_post_datetime")]
        public bool IsXPostDatetime { get; set; }
    }

    public class AnalysisDataPath
    {
        [JsonProperty("property")]
        public string Property { get; set; }
        
        [JsonProperty("analyser_id")]
        public string AnalyserComponentId { get; set; }
    }

    public enum ChartType
    {
        Line,
        Pie,
        Bar,
        Table,
        WordCloud,
        Scatter,
        
        PostsFrequency,
        LanguageFrequency,
        AuthorFrequency
    }
}

using System.Collections.Generic;

namespace Socneto.Api.Models
{
    public class CreateChartDefinitionRequest
    {
        public List<string> JsonDataPaths { get; set; }
        public string ChartType { get; set; }
    }
}
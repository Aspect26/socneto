using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class ChartDefinition
    {
        public List<string> JsonDataPaths { get; set; }
        public ChartType ChartType { get; set; }
    }

    public enum ChartType
    {
        Line,
        Pie
    }
}
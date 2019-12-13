using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class ChartDefinition
    {
        public List<AnalysisDataPath> AnalysisDataPaths { get; set; }
        public ChartType ChartType { get; set; }
    }

    public class AnalysisDataPath
    {
        public AnalysisProperty Property { get; set; }
        // TODO: why do we need this?
        public SocnetoComponent Analyser { get; set; }
    }

    public class AnalysisProperty
    {
        public string Name { get; set; }
        public AnalysisPropertyType Type { get; set; }    
    }

    public enum ChartType
    {
        Line,
        Pie,
        Scatter
    }

    public enum AnalysisPropertyType
    {
        Number,
        String,
        NumberList,
        StringList
    }
}
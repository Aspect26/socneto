using System.Collections.Generic;
using Socneto.Domain.Models;


namespace Socneto.Api.Models
{
    public enum AnalysisPropertyType
    {
        Number, String, List, Tuple
    }

    public class AnalysisProperty
    {
        public string Name { get; set; }
        
        public AnalysisPropertyType Type { get; set; }
    }
    
    public class AnalyserDto
    {
        public string Identifier { get; set; }
        
        public IList<AnalysisProperty> AnalysisProperties { get; set; }

        public static AnalyserDto FromModel(SocnetoComponent model)
        {
            return new AnalyserDto
            {
                Identifier = model.Id,
                AnalysisProperties = model.Attributes["analysis_properties"].ToObject<List<AnalysisProperty>>()
            };
        }
        
    }
}

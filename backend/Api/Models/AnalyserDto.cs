using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AnalyserDto
    {
        
        public string Identifier { get; set; }
        
        public IList<string> AnalysisProperties { get; set; }

        public static AnalyserDto FromModel(SocnetoComponent model)
        {
            return new AnalyserDto
            {
                Identifier = model.Id,
                AnalysisProperties = new List<string>()
            };
        }
        
    }
}

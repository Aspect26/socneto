using System;
using System.Collections.Generic;

namespace Socneto.Domain.Models
{
    public class AnalyzedPost
    {
        public int Id { get; set; }
        
        public Guid JobId { get; set; }

        public Post Post { get; set; }

        public Dictionary<string,  Dictionary<string, AnalysisValue>>[] Analyses { get; set; }
    }
}
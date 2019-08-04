using System;
using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AnalyzedPostDto
    {
        public Guid JobId { get; set; }

        public Post Post { get; set; }

        // TODO Julius : analyses were changed to 
        public Dictionary<string,  Dictionary<string, AnalysisValue>>[] Analyses { get; set; }

        public static AnalyzedPostDto FromModel(AnalyzedPost post)
        {
            return new AnalyzedPostDto
            {
                JobId =  post.JobId,
                Post = post.Post,
                Analyses = post.Analyses,
            };
        }
    }
}
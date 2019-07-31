using System;
using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AnalyzedPostDto
    {
        public Guid JobId { get; set; }

        public Post Post { get; set; }

        public Dictionary<string,  Dictionary<string, AnalysisValue>> Analysis { get; set; }

        public static AnalyzedPostDto FromModel(AnalyzedPost post)
        {
            return new AnalyzedPostDto
            {
                JobId =  post.JobId,
                Post = post.Post,
                Analysis = post.Analysis,
            };
        }
    }
}
using System;

namespace Domain.Model
{
    public class AnalysisResponse
    {
        public string ComponentId { get; set; }
        public string PostId { get; set; }
        public Guid JobId { get; set; }
        public Analysis Analysis { get; set; }

        public static AnalysisResponse FromData(string componentId, UniPost post, Analysis analysis)
        {
            return new AnalysisResponse
            {
                Analysis = analysis,
                PostId = post.PostId,
                JobId = post.JobId,
                ComponentId = componentId
            };
        }
    }
}
using System.Collections.Generic;

namespace Domain.Models
{
    public class AnalysisResult
    {
        public IList<AnalysedPostData> AnalysedPostDataList { get; set; }
        public IList<AnalysedUserData> AnalysedUserDataList { get; set; }
        public string[] Keywords { get; set; }
    }
}
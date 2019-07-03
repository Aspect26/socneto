using System.Collections.Generic;

namespace Api.Models
{
    public class JobSubmitRequest
    {
        public List<string> SelectedAnalysers { get; set; }
        public List<string> SelectedNetworks { get; set; }
        public string TopicQuery { get; set; }
    }
}
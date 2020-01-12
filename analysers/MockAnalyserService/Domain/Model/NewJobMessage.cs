using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Domain.Model
{
    public class NewJobMessage
    {
        public Guid JobId { get; set; }

        public IList<string> OutputChannelNames { get; set; }

        public JObject Attributes { get; set; }
    }
}
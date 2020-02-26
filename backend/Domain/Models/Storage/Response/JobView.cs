using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Socneto.Domain.Models.Storage.Response
{
    public class JobView
    {
        public Guid JobId { get; set; }

        [JsonProperty("viewConfiguration")]
        public ViewConfiguration ViewConfiguration { get; set; }
    }

    public class ViewConfiguration
    {
        [JsonProperty("chartDefinitions")]
        public IList<ChartDefinition> ChartDefinitions { get; set; }
    }
}

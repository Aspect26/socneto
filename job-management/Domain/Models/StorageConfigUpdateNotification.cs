using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class StorageConfigUpdateNotification
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }
    }
}
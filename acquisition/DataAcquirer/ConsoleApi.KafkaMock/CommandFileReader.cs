using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.JobConfiguration;
using Newtonsoft.Json;

namespace ConsoleApi.KafkaMock
{
    public class CommandFileReader
    {
        public async Task<IEnumerable<DataAcquirerCommand>> ReadCommandsAsync(string filePath)
        {
            using(var reader  = new StreamReader(filePath))
            {
                var fileContent = await reader.ReadToEndAsync();
                var lines = fileContent
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines
                    
                    .Select(r => JsonConvert.DeserializeObject<DataAcquirerCommand>(r))
                    .ToList();
            }
        }


    }

    public class DataAcquirerCommand
    {
        [JsonProperty("jobId")]
        public Guid JobId { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("attributes")]
        public Dictionary<string, string> Attributes { get; set; }

        [JsonProperty("outputMessageBrokerChannels")]
        public string[] OutputMessageBrokerChannels { get; set; }

        [JsonProperty("analysisOutputChannel")]
        public string AnalysisOutputChannel { get; set; }
    }
}

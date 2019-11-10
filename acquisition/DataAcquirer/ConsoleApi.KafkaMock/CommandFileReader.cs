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
        public async Task<IEnumerable<DataAcquirerJobConfig>> ReadCommandsAsync(string filePath)
        {
            using(var reader  = new StreamReader(filePath))
            {
                var fileContent = await reader.ReadToEndAsync();
                var lines = fileContent
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines
                    
                    .Select(r => JsonConvert.DeserializeObject<DataAcquirerJobConfig>(r))
                    .ToList();
            }
        }


    }
}

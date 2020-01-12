using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApi.JobInvoker
{
    public class CommandFileReader
    {
        public async Task<IEnumerable<T>> ReadCommandsAsync<T>(string filePath)
        {
            using(var reader  = new StreamReader(filePath))
            {
                var fileContent = await reader.ReadToEndAsync();
                var lines = fileContent
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines
                    
                    .Select(r => JsonConvert.DeserializeObject<T>(r))
                    .ToList();
            }
        }
    }
}

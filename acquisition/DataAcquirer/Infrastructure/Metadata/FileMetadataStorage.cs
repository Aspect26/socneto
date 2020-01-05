using System;
using System.IO;
using System.Threading.Tasks;
using Domain.Acquisition;
using Domain.JobManagement;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Metadata
{
    public class FileMetadataStorage : IDataAcquirerMetadataStorage
    {
        private readonly FileJsonStorageOptions _options;

        public FileMetadataStorage(
            IOptions<FileJsonStorageOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task<T> GetAsync<T>(Guid jobId)
            where T : class, IDataAcquirerMetadata
        {
            var path = ResolvePath(jobId);
            string objJson;
            using (var sw = new StreamReader(path))
            {
                objJson = await sw.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<T>(objJson);
        }

        public async Task SaveAsync<T>(Guid jobId, T metadata)
            where T : class, IDataAcquirerMetadata
        {
            var path = ResolvePath(jobId);
            var obj = JsonConvert.SerializeObject(metadata);
            using var sw = new StreamWriter(path);
            await sw.WriteAsync(obj);
        }

        private string ResolvePath(Guid id)
        {
            var fileName = string.Format(_options.FilePathTemplate, id.ToString());
            return Path.Combine(_options.Directory, fileName);
        }
    }
}

using Domain.Acquisition;
using Domain.JobManagement;
using Infrastructure.Twitter;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Metadata;
using Newtonsoft.Json;

namespace ConsoleApi.Twitter
{
    public class TestingFileMetadataStorage : IDataAcquirerMetadataStorage
    {
        private readonly FileJsonStorageOptions _options;
        private readonly TwitterMetadata _metadata;

        public TestingFileMetadataStorage(
            IOptions<TwitterMetadata> metadata,
            IOptions<FileJsonStorageOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
            _metadata = metadata.Value;
        }

        public async Task<T> GetAsync<T>(Guid jobId)
            where T : class, IDataAcquirerMetadata
        {
            var path = ResolvePath(jobId);
            string objJson;
            try
            {
                using (var sw = new StreamReader(path))
                {
                    objJson = await sw.ReadToEndAsync();
                }
            }
            catch (IOException)
            {
                if(_metadata is T twMeta)
                {
                    return twMeta;
                }

                return null;
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

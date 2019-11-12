using Domain.Acquisition;
using Domain.JobManagement;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Twitter
{
    public class TwitterJsonFileMetadataStorage : IDataAcquirerMetadataStorage
    {
        private readonly TwitterJsonStorageOptions _twitterJsonStorageOptions;
        private readonly DirectoryInfo _baseDirectory;
        private object _rwLock = new object();
        public TwitterJsonFileMetadataStorage(
            IOptions<TwitterJsonStorageOptions> options)
        {
            _twitterJsonStorageOptions = options.Value;
            _baseDirectory = new DirectoryInfo(_twitterJsonStorageOptions.Directory);
        }

        public Task<IDataAcquirerMetadata> GetAsync(Guid jobId)
        {
            
            var filePath = string.Format(_twitterJsonStorageOptions.FilePathTemplate, jobId);

            var fullName = Path.Combine(_baseDirectory.FullName, filePath);

            try
            {
                lock (_rwLock)
                {
                    using var reader = new StreamReader(fullName);
                    var metadata = reader.ReadToEnd();
                    try
                    {
                        var twitterMeta= JsonConvert.DeserializeObject<TwitterMetadata>(metadata);
                        return Task.FromResult<IDataAcquirerMetadata>(twitterMeta);

                    }
                    catch (JsonReaderException)
                    {
                        throw new InvalidOperationException($"Invalid format of json {metadata}");
                    }
                }
            }
            catch (IOException)
            {
                return Task.FromResult<IDataAcquirerMetadata>(null);
            }
        }

        public Task SaveAsync(Guid jobId, IDataAcquirerMetadata defaultMetadata)
        {
            var filePath = string.Format(_twitterJsonStorageOptions.FilePathTemplate, jobId);
            var fullName = Path.Combine(_baseDirectory.FullName, filePath);
            lock (_rwLock)
            {
                using var writer = new StreamWriter(fullName);
                var metadata = JsonConvert.SerializeObject(defaultMetadata);
                writer.WriteLine(metadata);
            }

            return Task.CompletedTask;
        }
    }
}

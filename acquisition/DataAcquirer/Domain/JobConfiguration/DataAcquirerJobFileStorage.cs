using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.JobConfiguration
{
    public class DataAcquirerJobFileStorage : IDataAcquirerJobStorage
    {
        private readonly DataAcquirerJobFileStorageOptions _redditStorage;
        private object _rwLock = new object();
        public DataAcquirerJobFileStorage(
            IOptions<DataAcquirerJobFileStorageOptions> options)
        {
            _redditStorage = options.Value;
        }

        public async Task<IList<DataAcquirerJobConfig>> GetAllAsync()
        {
            var directory = new DirectoryInfo(_redditStorage.Directory);
            var files = directory.GetFiles(_redditStorage.FilePathPrefix + "*");

            var loadTasks = files.Select(r => GetByPathAsync(r.FullName));

            await Task.WhenAll(loadTasks);
            return loadTasks.Select(r => r.Result).ToList();
        }

        public Task RemoveJobAsync(Guid jobId)
        {
            var directory = new DirectoryInfo(_redditStorage.Directory);
            var fileName = _redditStorage.FilePathPrefix + "_" + jobId;
            var file = Path.Combine(directory.FullName, fileName);
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            { }

            return Task.CompletedTask;
        }

        private Task<DataAcquirerJobConfig> GetByPathAsync(string filePath)
        {
            try
            {
                lock (_rwLock)
                {
                    using var reader = new StreamReader(filePath);
                    var jobJson = reader.ReadToEnd();
                    try
                    {
                        var twitterMeta = JsonConvert.DeserializeObject<DataAcquirerJobConfig>(jobJson);
                        return Task.FromResult(twitterMeta);

                    }
                    catch (JsonReaderException)
                    {
                        throw new InvalidOperationException($"Invalid format of json {jobJson}");
                    }
                }
            }
            catch (IOException)
            {
                throw new InvalidOperationException($"Could not process file {filePath}");
            }
        }

        public Task SaveAsync(Guid jobId, DataAcquirerJobConfig jobConfig)
        {
            var directory = new DirectoryInfo(_redditStorage.Directory);
            var fileName = _redditStorage.FilePathPrefix + "_" + jobId;
            var file = Path.Combine(directory.FullName, fileName);

            lock (_rwLock)
            {
                using var writer = new StreamWriter(file);
                var metadata = JsonConvert.SerializeObject(jobConfig);
                writer.WriteLine(metadata);
            }

            return Task.CompletedTask;
        }

    }

}

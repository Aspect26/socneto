using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Twitter
{

    //[Obsolete("use storage proxy instead",error:true)]
    //public class TwitterJsonFileMetadataStorage : IDataAcquirerMetadataStorage
    //{
    //    private readonly TwitterJsonStorageOptions _twitterJsonStorageOptions;
    //    private readonly DirectoryInfo _baseDirectory;
    //    private readonly object _rwLock = new object();
    //    public TwitterJsonFileMetadataStorage(
    //        IOptions<TwitterJsonStorageOptions> options)
    //    {
    //        _twitterJsonStorageOptions = options.Value;
    //        _baseDirectory = new DirectoryInfo(_twitterJsonStorageOptions.Directory);
    //    }

    //    public Task<T> GetAsync<T>(Guid jobId) where T : class,IDataAcquirerMetadata
    //    {

    //        var filePath = string.Format(_twitterJsonStorageOptions.FilePathTemplate, jobId);

    //        var fullName = Path.Combine(_baseDirectory.FullName, filePath);

    //        try
    //        {
    //            lock (_rwLock)
    //            {
    //                using var reader = new StreamReader(fullName);
    //                var metadata = reader.ReadToEnd();
    //                try
    //                {
    //                    var twitterMeta = JsonConvert.DeserializeObject<T>(metadata);
    //                    return Task.FromResult(twitterMeta);

    //                }
    //                catch (JsonReaderException)
    //                {
    //                    throw new InvalidOperationException($"Invalid format of json {metadata}");
    //                }
    //            }
    //        }
    //        catch (IOException)
    //        {
    //            return Task.FromResult<T>(null);
    //        }
    //    }

    //    public Task SaveAsync<T>(T metadata) where T :class, IDataAcquirerMetadata
    //    {
    //        var filePath = string.Format(
    //            _twitterJsonStorageOptions.FilePathTemplate, 
    //            metadata.JobId);
    //        var fullName = Path.Combine(_baseDirectory.FullName, filePath);
    //        lock (_rwLock)
    //        {
    //            using var writer = new StreamWriter(fullName);
    //            var metadataJson = JsonConvert.SerializeObject(metadata);
    //            writer.WriteLine(metadataJson);
    //        }

    //        return Task.CompletedTask;
    //    }
    //}
}

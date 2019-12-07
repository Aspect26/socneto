using System.Threading.Tasks;

namespace Domain.Acquisition
{
    
    public class NullContext : IDataAcquirerMetadataContext
    {
        public Task<T> GetOrCreateAsync<T>(T defaultIfNew)
            where T : class, IDataAcquirerMetadata
        {
            return Task.FromResult(default(T));
        }

        public Task UpdateAsync<T>(T metadata)
            where T : class, IDataAcquirerMetadata
        {
            return Task.CompletedTask;
        }
    }




}

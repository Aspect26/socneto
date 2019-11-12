using System.Threading.Tasks;

namespace Domain.Acquisition
{
    public class NullContext : IDataAcquirerMetadataContext
    {
        public Task<T> GetOrCreateAsync<T>(T defaultIfNew) where T : IDataAcquirerMetadata
        {
            return Task.FromResult(defaultIfNew);
        }

        public Task UpdateAsync(IDataAcquirerMetadata metadata)
        {
            return Task.CompletedTask;
        }
    }

    


}

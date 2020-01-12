using System.Threading.Tasks;

namespace Domain.Acquisition
{
    public interface IDataAcquirerMetadataContext
    {

        Task<T> GetOrCreateAsync<T>(T defaultIfNew) where T : class, IDataAcquirerMetadata;
        Task UpdateAsync<T>(T metadata) where T : class, IDataAcquirerMetadata;
    }
}

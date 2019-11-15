﻿using System.Threading.Tasks;

namespace Domain.Acquisition
{
    public interface IDataAcquirerMetadataContext
    {
        Task<T> GetOrCreateAsync<T>(T defaultIfNew) where T :IDataAcquirerMetadata;
        Task UpdateAsync(IDataAcquirerMetadata metadata);
    }

    


}
using System;

namespace Domain.Model
{
    public class DataAcquirerInputModel
    {
        public DataAcquirerInputModel(
            Guid jobId,
            string query,
            string queryLanguage,
            DataAcquirerAttributes attributes,
            ulong latestRecordId,
            ulong earliestRecordId,
            int batchSize
            )
        {
            Attributes = attributes;

            JobId = jobId;
            Query = query;
            QueryLanguage = queryLanguage;
            LatestRecordId = latestRecordId;
            EarliestRecordId = earliestRecordId;
            BatchSize = batchSize;
        }
        public DataAcquirerAttributes Attributes { get;  }

        public string Query { get;  }
        public string QueryLanguage { get; }
        public Guid JobId { get;  }
        
        public ulong EarliestRecordId { get; }
        public ulong LatestRecordId { get; }
        public int BatchSize { get; }

        public static DataAcquirerInputModel FromValues(
            Guid jobId,
            string query,
            string queryLanguage,
            DataAcquirerAttributes attributes,
            ulong latestRecordId,
            ulong earliestRecordId,
            int numberOfPostToRetreive)
        {
            return new DataAcquirerInputModel(
            jobId,
            query,
            queryLanguage,
            attributes,
            latestRecordId,
            earliestRecordId,
            numberOfPostToRetreive);
        }
    }
}

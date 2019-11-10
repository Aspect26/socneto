using System.Collections.Generic;

namespace Domain.Model
{
    public class DataAcquirerOutputModel
    {
        public DataAcquirerOutputModel(
            IList<UniPost> posts,
            ulong minId,
            ulong maxId)
        {
            Posts = posts;
            LatestRecordId = minId;
            EarliestRecordId = maxId;
        }
        public IList<UniPost> Posts { get; }
        public ulong EarliestRecordId { get; }
        public ulong LatestRecordId { get; }

    }
}

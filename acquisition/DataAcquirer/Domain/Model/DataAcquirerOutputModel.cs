using System.Collections.Generic;

namespace Domain.Model
{
    public class DataAcquirerOutputModel
    {
        public IList<UniPost> Posts { get; set; }
        public ulong MaxId { get; set; }
    }
}
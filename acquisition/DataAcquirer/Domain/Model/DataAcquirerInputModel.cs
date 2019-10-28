using System;

namespace Domain.Model
{
    public class DataAcquirerInputModel
    {
        public DataAcquirerAttributes Attributes { get; set; }

        public string Query { get; set; }
        public Guid JobId { get; set; }
        
        public ulong FromId { get; set; }
        public int NumberOfPostToRetrieve { get; set; }
    }
}
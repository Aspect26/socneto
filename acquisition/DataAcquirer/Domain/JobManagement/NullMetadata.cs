using System;
using Domain.Acquisition;

namespace Domain.JobManagement
{
    public class NullMetadata : IDataAcquirerMetadata
    {
        public Guid JobId { get; set; }
    }




}

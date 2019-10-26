using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.JobManagement
{
    public class JobManagerJobRecord
    {
        public Guid JobId { get; set; }
        public Task JobTask { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
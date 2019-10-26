using System;

namespace Domain.SubmittedJobConfiguration
{
    public class JobConfigUpdateResult
    {
        public bool HasError { get; }
        public string Error { get; }
        public Guid JobId { get;  }
        public JobStatus Status { get;  }

        private JobConfigUpdateResult(string error)
        {
            Error = error;
            HasError = true;
        }

        private JobConfigUpdateResult(Guid jobId, JobStatus jobStatus)
        {
            JobId = jobId;
            Status = jobStatus;
        }

        public static JobConfigUpdateResult Successfull(Guid jobId, JobStatus status)
        {
            return new JobConfigUpdateResult(jobId, status);
        }

        public static JobConfigUpdateResult Failed(string error)
        {
            return new JobConfigUpdateResult(error);
        }
    }
}
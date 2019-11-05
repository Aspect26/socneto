using System;

namespace Infrastructure
{
    public class JobConfigStorageOptions
    {
        public string BaseUri { get; set; }

        public string AddJobConfigRoute { get; set; }
        public string UpdateJobConfigRoute { get; set; }
        public string GetJobConfigRoute { get; set; }
    }
}

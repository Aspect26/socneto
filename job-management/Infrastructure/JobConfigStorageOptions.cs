using System;

namespace Infrastructure
{
    public class JobStorageOptions
    {
        public Uri BaseUri { get; set; }

        public string AddJobRoute { get; set; }
        public string UpdateJobRoute { get; set; }
        public string GetJobRoute { get; set; }

        public string AddJobConfigRoute { get; set; }
    }
}

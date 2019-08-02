using Domain.JobConfiguration;

namespace Domain
{
    public interface IJobConfigService
    {
        void SetCurrentConfig(DataAnalyzerJobConfig jobConfig);

        DataAnalyzerJobConfig GetCurrentConfig();
    }

    public class JobConfigService : IJobConfigService
    {
        private DataAnalyzerJobConfig _config;
        
        public void SetCurrentConfig(DataAnalyzerJobConfig jobConfig)
        {
            _config = jobConfig;
        }

        public DataAnalyzerJobConfig GetCurrentConfig()
        {
            return _config;
        }
    }
}
using System.Threading.Tasks;

namespace Domain
{
    public interface ITaskService
    {
        void StartTaskConsumeTasks();
        Task StopTaskConsumeTaskAsync();
    }
}
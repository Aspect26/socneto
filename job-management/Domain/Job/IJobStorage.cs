using System.Threading.Tasks;

namespace Domain.Job
{
    public interface IJobStorage
    {
        
        Task InsertNewJobAsync(Models.Job job);

    }
}
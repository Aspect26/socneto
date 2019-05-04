
namespace Socneto.Coordinator.Domain.Models
{
    public class UserData
    {
        public string Name { get; set; }
    }

    public class JobDataRequest
    {
        public string  JobId { get; set; }
        public string Topic { get; set; }
        
    }
}
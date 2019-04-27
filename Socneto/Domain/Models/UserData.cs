
namespace Socneto.Coordinator.Domain.Models
{
    public class UserData
    {
        public string Name { get; set; }
    }

    public class JobResultWrapper
    {
        public string  JobId { get; set; }
        public string DataType { get; set; }
        public object Data { get; set; }
    }
}
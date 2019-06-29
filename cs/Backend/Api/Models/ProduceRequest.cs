namespace Socneto.Api.Models
{
    public class ProduceRequest
    {

        public string ServerAddress { get; set; }
        public string KafkaTopic { get; set; }

        public string Message { get; set; }
    }
}
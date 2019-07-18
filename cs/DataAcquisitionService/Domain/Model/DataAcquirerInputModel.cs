namespace Domain.Model
{
    public class DataAcquirerInputModel
    {
        public DataAcquirerCredentials NetworkCredentials { get; set; }

        public string Query { get; set; }
    }
}
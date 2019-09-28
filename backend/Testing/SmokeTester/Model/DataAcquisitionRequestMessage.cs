using System;

namespace SmokeTester.Model
{
    public class DataAcquisitionRequestMessage
    {
        public Guid JobId { get; set; }
        public string Query { get; set; }
        
    }
}
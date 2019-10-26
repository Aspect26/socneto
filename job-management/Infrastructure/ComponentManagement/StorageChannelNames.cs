using System.ComponentModel.DataAnnotations;

namespace Infrastructure.ComponentManagement
{
    public class StorageChannelNames
    {
        [Required]
        public string StoreAnalysedDataChannelName { get; set; }
        [Required]
        public string StoreRawDataChannelName { get; set; }
    }
}
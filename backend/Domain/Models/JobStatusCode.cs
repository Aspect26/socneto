using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Domain.Models
{
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobStatusCode
    {
        
        [EnumMember(Value = "Running")]
        Running,
        
        [EnumMember(Value = "Stopped")]
        Stopped,
    }
}
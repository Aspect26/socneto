using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Socneto.Api.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocnetoComponentStatus
    {
        [EnumMember(Value = "STOPPED")]
        Stopped,
        
        [EnumMember(Value = "STARTING")]
        Starting,
        
        [EnumMember(Value = "RUNNING")]
        Running
    }
}
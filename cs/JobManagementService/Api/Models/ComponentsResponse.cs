using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class ComponentsResponse
    {

        public ComponentsResponse(
            string componenId,
            string componentType)
        {
            ComponentType = componentType;
            ComponenId = componenId;
        }
        public string ComponentType { get;  }
        public string ComponenId { get;  }
    }
}

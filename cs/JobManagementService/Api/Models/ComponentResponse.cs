using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class ComponentsResponse
    {

        public ComponentsResponse(
            string componentId,
            string componentType)
        {
            ComponentType = componentType;
            ComponentId = componentId;
        }
        public string ComponentType { get;  }
        public string ComponentId { get;  }
    }
}

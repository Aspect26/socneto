using System.Collections.Generic;

namespace Api.Models
{
    public class ComponentListResponse
    {
        public ComponentListResponse(IList<ComponentsResponse> components)
        {
            Components = components;
        }
        public IList<ComponentsResponse> Components { get; }
    }
}
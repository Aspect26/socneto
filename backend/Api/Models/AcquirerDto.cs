using System.Collections.Generic;
using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class AcquirerDto
    {
        
        public string Identifier { get; set; }
        
        public IList<string> Properties { get; set; }

        public static AcquirerDto FromModel(SocnetoComponent model)
        {
            return new AcquirerDto
            {
                Identifier = model.Id,
                Properties = new List<string>()
            };
        }
        
    }
}

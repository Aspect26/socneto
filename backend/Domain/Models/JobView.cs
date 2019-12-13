using System;
using System.Collections.Generic;


namespace Socneto.Domain.Models
{
    public class JobView
    {
        public Guid JobId { get; set; }

        public IList<ChartDefinition> ViewConfiguration { get; set; }
    }    
}

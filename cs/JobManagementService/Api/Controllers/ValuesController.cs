using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class ComponentController: ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<string>> GetAvailableComponents()
        {
            // TODO
            return new string[] { "value1", "value2" };
        }
    }
}

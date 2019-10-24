using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Socneto.Api.Models;
using Socneto.Domain.Services;

namespace Socneto.Api.Controllers
{
    
    [Authorize]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        
        private IStorageService _storageService;
        private ILogger<ComponentsController> _logger;

        public ComponentsController(IStorageService storageService, ILogger<ComponentsController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("api/components/analysers")]
        public async Task<ActionResult<List<AnalyserDto>>> GetAnalysers()
        {
            var analysers = await _storageService.GetAnalysers();
            var analysersDto = analysers.Select(AnalyserDto.FromModel).ToList();
            
            return Ok(analysersDto);
        }

        [HttpGet]
        [Route("api/components/acquirers")]
        public async Task<ActionResult<List<AcquirerDto>>> GetAcquirers()
        {
            var acquirers = await _storageService.GetAcquirers();
            var acquirersDto = acquirers.Select(AcquirerDto.FromModel).ToList();

            return Ok(acquirersDto);
        }
        
    }
}

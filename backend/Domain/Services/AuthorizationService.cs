using System;
using System.Threading.Tasks;

namespace Socneto.Domain.Services
{
    public class AuthorizationService : IAuthorizationService
    {

        private readonly IStorageService _storageService;

        public AuthorizationService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public async Task<bool> IsUserAuthorizedToSeeJob(string username, Guid jobId)
        {
            var job = await _storageService.GetJob(jobId);
            return job.Username == username;
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Socneto.Domain.Services
{
    public interface IAuthorizationService
    {
        Task<bool> IsUserAuthorizedToSeeJob(string username, Guid jobId);
    }
}
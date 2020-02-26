using System.Threading.Tasks;
using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Domain.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }
}
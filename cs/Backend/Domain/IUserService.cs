using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }
}
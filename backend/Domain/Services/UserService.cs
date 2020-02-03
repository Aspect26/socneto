using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IStorageService _storageService;

        public UserService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public async Task<User> Authenticate(string username, string password)
        {
            var user = await _storageService.GetUser(username);
            if (user == null || user.Password != password)
                return null;

            user.Password = null;
            return user;
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Socneto.Domain.Models;

namespace Socneto.Domain.Services
{
    public class UserService : IUserService
    {
        // TODO: read this from DB
        private readonly List<User> _users = new List<User>
        { 
            new User { Id = 1, Username = "admin", Password = "admin" } 
        };
        
        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));

            if (user == null)
                return null;

            // authentication successful so return user details without password
            user.Password = null;
            
            return user;
        }

        public async Task<User> GetUserByName(string username)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username));
            return user;
        }
    }
}
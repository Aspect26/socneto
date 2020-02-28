using Socneto.Domain.Models;
using Socneto.Domain.Models.Storage.Response;

namespace Socneto.Api.Models.Responses
{
    public class LoginResponse
    {
        public string Username { get; set; }
        
        public static LoginResponse FromModel(User user)
        {
            return new LoginResponse
            {
                Username = user.Username,
            };
        }
    }
}
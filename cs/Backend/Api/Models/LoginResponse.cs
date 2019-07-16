using Socneto.Domain.Models;

namespace Socneto.Api.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }

        public static LoginResponse FromModel(User user)
        {
            return new LoginResponse()
            {
                Username = user.Username
            };
        }
    }
}
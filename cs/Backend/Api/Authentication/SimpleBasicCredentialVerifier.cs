using System.Threading.Tasks;
using Bazinga.AspNetCore.Authentication.Basic;

namespace Socneto.Api.Authentication
{
    
    public class SimpleBasicCredentialVerifier : IBasicCredentialVerifier
    {
        public Task<bool> Authenticate(string username, string password)
        {
            // TODO: remove this when FE stores credentials in localstorage
            return Task.FromResult(true);
            // return Task.FromResult(username == "admin" && password == "admin");
        }
    }
}
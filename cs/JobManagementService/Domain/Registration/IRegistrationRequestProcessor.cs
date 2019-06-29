using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Registration
{
    public interface IRegistrationRequestProcessor
    {
        Task ProcessRequest(RegistrationRequestMessage request);
    }
}
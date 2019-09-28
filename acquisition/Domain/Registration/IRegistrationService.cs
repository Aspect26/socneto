using System.Threading.Tasks;

namespace Domain.Registration
{
    public interface IRegistrationService
    {
        Task Register(RegistrationRequest registrationRequest);
    }
}
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain
{
    public interface ISubscribedComponentManager
    {
        void SubscribeComponent(ComponentRegistrationModel componentRegistrationModel);

        // todo
        // Task PushJobConfiguration();
    }


    
}

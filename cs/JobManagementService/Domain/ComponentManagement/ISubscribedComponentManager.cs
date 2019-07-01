using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;

namespace Domain.ComponentManagement
{
    public interface ISubscribedComponentManager
    {
        void SubscribeComponent(ComponentRegistrationModel componentRegistrationModel);

        Task PushJobConfigUpdateAsync(JobConfigUpdateNotification jobConfigUpdateNotification);
        IList<SubscribedComponent> GetAvaliableNetworks();

        IList<SubscribedComponent> GetAvaliableAnalysers();
    }
}

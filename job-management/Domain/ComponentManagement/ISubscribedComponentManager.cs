using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.SubmittedJobConfiguration;

namespace Domain.ComponentManagement
{
    public interface ISubscribedComponentManager
    {
        Task<SubscribedComponentResultModel> SubscribeComponentAsync(ComponentRegistrationModel componentRegistrationModel);

        Task PushJobConfigUpdateAsync(JobConfigUpdateNotification jobConfigUpdateNotification);
    }

    public enum SubscribedComponentResult
    {
        AlreadyExists, Successful, Failed
    }

}

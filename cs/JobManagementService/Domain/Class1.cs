using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Extensions.Hosting;

namespace Domain
{
    public class RequestListenerHostedService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class RequestListener : IRequestListener
    {

    }

    public interface IRequestListener
    {

    }

   

    public interface IRegistrationRequestProcessor
    {
        void ProcessRequest(RegistrationRequestMessage request);
    }
    
    public interface IMessageBrokerApi
    {
        CreateChannelResult CreateChannel(MessageBrokerChannelModel channelModel);

    }

    public class CreateChannelResult
    {
        public CreateChannelResult(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get;  }
    }

    
    public interface IComponentConfigSubscriber
    {
        void SubscribeComponent(ComponentRegisterModel componentRegisterModel);
    }



}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class MessageBrokerChannelModel
    {

        public string ComponentId { get; }

        public string UpdateChannelName { get; }

        public MessageBrokerChannelModel(string componentId, string configUpdateChannelName)
        {
            ComponentId = componentId;
            UpdateChannelName = configUpdateChannelName;
        }

        public static MessageBrokerChannelModel FromRequest(RegistrationRequestMessage request)
        {
            return new MessageBrokerChannelModel(
                request.ComponentId,
                request.UpdateChannelName
            );
        }
    }
}

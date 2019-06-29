using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class MessageBrokerChannelModel
    {

        public string ComponentId { get; }

        public MessageBrokerChannelModel(string componentId)
        {
            ComponentId = componentId;
            
        }

        public static MessageBrokerChannelModel FromRequest(RegistrationRequestMessage request)
        {
            return new MessageBrokerChannelModel(
                request.ComponentId
            );
        }
    }
}

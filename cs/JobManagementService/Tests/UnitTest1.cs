using Domain;
using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RequestListenerTest()
        {
            
            IRequestListener requestListener = new RequestListener();

           
        }

        [TestMethod]
        public void RequestProcessorTest()
        {
            // Arrange
            var componentConfigSub = new Mock<IComponentConfigSubscriber>();
            var messageBrokerApi = new Mock<IMessageBrokerApi>();
            messageBrokerApi.Setup(mba =>
                mba.CreateChannel(It.IsAny<MessageBrokerChannelModel>()))
            .Returns(new CreateChannelResult("config_distribution.analyser_test_id.configuration") );

            IRegistrationRequestProcessor registrationRequestProcessor 
                = new RegistrationRequestProcessor(
                    componentConfigSub.Object,
                    messageBrokerApi.Object);

            // Act
            var componentType = "analyser";
            var componentId = "analyser_test_id";
            var request = new RegistrationRequestMessage(
                componentId,
                componentType);
            

            registrationRequestProcessor.ProcessRequest(request);

            // Assert
            messageBrokerApi.Verify(mba => 
                mba.CreateChannel(
                    It.IsAny<MessageBrokerChannelModel>()), 
                    Times.Once());

            componentConfigSub.Verify(ccs =>
                ccs.SubscribeComponent(
                    It.IsAny<ComponentRegisterModel>()),
                    Times.Once());
        }

        // What if I register sth that was already registered?

    }
}

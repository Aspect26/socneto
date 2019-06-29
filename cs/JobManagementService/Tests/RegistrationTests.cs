using System;
using Domain;
using Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class RegistrationTests
    {
        [TestMethod]
        public void RequestListenerTest()
        {
            IRequestListener requestListener = new RequestListener();
        }
        
        [TestMethod]
        public void RequestIntegrationTest()
        {
            // Arrange
            var componentRegistry = new ComponentRegistry();
            var subscribedComponentManager = new SubscribedComponentManager(componentRegistry);


            var messageBrokerApi = new Mock<IMessageBrokerApi>();
            messageBrokerApi.Setup(mba =>
                mba.CreateChannel(It.IsAny<MessageBrokerChannelModel>()))
            .Returns(new CreateChannelResult("config_distribution.analyser_test_id.configuration") );

            IRegistrationRequestProcessor registrationRequestProcessor 
                = new RegistrationRequestProcessor(
                    subscribedComponentManager,
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

            var components = componentRegistry.GetAllByType("analyser");
            Assert.AreEqual(1,components.Count,"Exactly one component should be registered");

            Assert.ThrowsException<InvalidOperationException>(
                () => registrationRequestProcessor.ProcessRequest(request));
            
            Assert.AreEqual(1, components.Count, "Number of component must not change");
        }
    }

    
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Models;
using Domain.Registration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Tests.MockClasses;

namespace Tests
{
    [TestClass]
    public class RegistrationTests
    {
        [TestMethod]
        public void RequestListenerTest()
        {
            var messageBrokerConsumerMockObject = new MessageBrokerConsumerMock();

            var registrationRequestProcessorMock = new Mock<IRegistrationRequestProcessor>();

            var registrationOptions = Options.Create(new RegistrationRequestOptions
            {
                RegistrationChannelName = "testContext.testComponent.testMessageType"
            });

            var loggerMock = new Mock<ILogger<RegistrationRequestListener>>();

            var registrationRequestListener = new RegistrationRequestListener(
                registrationRequestProcessorMock.Object,
                messageBrokerConsumerMockObject,
                registrationOptions,
                loggerMock.Object);

            var cancellationTokenSource = new CancellationTokenSource();
            var listenTask =registrationRequestListener.Listen(cancellationTokenSource.Token);


            var testRegistrationRequest = new
            {
                componentType = "componentType1",
                componentId = "componentId1"
            };

            var requestJson = JsonConvert.SerializeObject(testRegistrationRequest);
            messageBrokerConsumerMockObject.OnRecieveAction(requestJson);

            Assert.AreEqual("testContext.testComponent.testMessageType",
                messageBrokerConsumerMockObject.ConsumeTopic);

            cancellationTokenSource.Cancel();
            listenTask.Wait();
            
            registrationRequestProcessorMock.Verify(

                rrp => rrp.ProcessRequest(
                    It.Is((RegistrationRequestMessage val) =>
                       val.ComponentType == testRegistrationRequest.componentType
                       && val.ComponentId == testRegistrationRequest.componentId)),
                    Times.Once()
                );
            registrationRequestProcessorMock.VerifyNoOtherCalls();

        }

        [TestMethod]
        public void RequestIntegrationTest()
        {
            // Arrange
            var componentRegistry = new ComponentRegistry();
            var subscribedComponentManager = new SubscribedComponentManager(componentRegistry);


            var messageBrokerApiMock = new Mock<IMessageBrokerApi>();
            messageBrokerApiMock.Setup(mba =>
                mba.CreateChannel(It.IsAny<MessageBrokerChannelModel>()))
            .Returns(
                    Task.FromResult(
                        new CreateChannelResult("config_distribution.analyser_test_id.configuration"))
                );

            IRegistrationRequestProcessor registrationRequestProcessor
                = new RegistrationRequestProcessor(
                    subscribedComponentManager,
                    messageBrokerApiMock.Object);

            // Act
            var componentType = "analyser";
            var componentId = "analyser_test_id";
            var request = new RegistrationRequestMessage
            {
                ComponentType = componentType,
                ComponentId = componentId
            };

            registrationRequestProcessor.ProcessRequest(request);

            // Assert
            messageBrokerApiMock.Verify(mba =>
                mba.CreateChannel(
                    It.IsAny<MessageBrokerChannelModel>()),
                    Times.Once());
            messageBrokerApiMock.VerifyNoOtherCalls();

            var components = componentRegistry.GetAllByType("analyser");
            Assert.AreEqual(1, components.Count, "Exactly one component should be registered");

            Assert.ThrowsException<InvalidOperationException>(
                () => registrationRequestProcessor.ProcessRequest(request));

            Assert.AreEqual(1, components.Count, "Number of component must not change");

        }
    }



}

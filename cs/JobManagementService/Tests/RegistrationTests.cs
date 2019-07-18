using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.ComponentManagement;
using Domain.Models;
using Domain.Registration;
using Domain.SubmittedJobConfiguration;
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

                rrp => rrp.ProcessRequestAsync(
                    It.Is((RegistrationRequestMessage val) =>
                       val.ComponentType == testRegistrationRequest.componentType
                       && val.ComponentId == testRegistrationRequest.componentId)),
                    Times.Once()
                );
            registrationRequestProcessorMock.VerifyNoOtherCalls();

        }

        [TestMethod]
        public async Task RequestIntegrationTest()
        {
            // Arrange
            var componentRegistry = new ComponentRegistry();
            var subscribedCompnentLogger = new Mock<ILogger<SubscribedComponentManager>>();
            var componentConfigNotifierMock = new Mock<IComponentConfigUpdateNotifier>();

            var componentOptions = Options.Create(new SubscribedComponentManagerOptions()
            {
                // TODO
            });

            var subscribedComponentManager = new SubscribedComponentManager(
                componentRegistry,
                componentConfigNotifierMock.Object,
                componentOptions,
                subscribedCompnentLogger.Object
            );


            var messageBrokerApiMock = new Mock<IMessageBrokerApi>();
            messageBrokerApiMock.Setup(mba =>
                mba.CreateChannel(It.IsAny<MessageBrokerChannelModel>()))
            .Returns(
                    Task.FromResult(
                        new CreateChannelResult("config_distribution.analyser_test_id.configuration"))
                );

            var registreationRequestProcessorLoggerMock = new Mock<ILogger<RegistrationRequestProcessor>>();
            IRegistrationRequestProcessor registrationRequestProcessor
                = new RegistrationRequestProcessor(
                    subscribedComponentManager,
                    messageBrokerApiMock.Object,
                    registreationRequestProcessorLoggerMock.Object
                    );

            // Act
            var componentType = "Analyser";
            var componentId = "analyser_test_id";
            var request = new RegistrationRequestMessage
            {
                ComponentType = componentType,
                ComponentId = componentId
            };

            await registrationRequestProcessor.ProcessRequestAsync(request);

            // Assert
            messageBrokerApiMock.Verify(mba =>
                mba.CreateChannel(
                    It.IsAny<MessageBrokerChannelModel>()),
                    Times.Once());
            messageBrokerApiMock.VerifyNoOtherCalls();


            Assert.IsTrue(componentRegistry.TryGetAnalyserComponent("analyser_test_id", out var cmp));
            Assert.AreEqual("Analyser",  cmp.ComponentType);
            
            try
            {
                await registrationRequestProcessor.ProcessRequestAsync(request);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // intentionally empty
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
        }
    }



}

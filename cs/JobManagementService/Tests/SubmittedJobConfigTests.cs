using System;
using System.Collections.Generic;
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
    public class SubmittedJobConfigTests
    {

        [TestMethod]
        public void SubmittedJobConfigListenerTest()
        {
            var messageBrokerConsumerMockObject = new MessageBrokerConsumerMock();

            var subscribedComponentManager
                = new Mock<ISubscribedComponentManager>();

            subscribedComponentManager.Setup(scm => scm
                    .PushJobConfigUpdateAsync(It.IsAny<JobConfigUpdateNotification>()))
                .Returns(Task.CompletedTask);

            var registrationOptions = Options.Create(new SubmittedJobConfigListenerOptions
            {
                JobConfigChannelName = "testContext.testComponent.testMessageType"
            });

            var loggerMock = new Mock<ILogger<SubmittedJobConfigListenerService>>();

            var submittedJobConfigListenerService = new SubmittedJobConfigListenerService(
                subscribedComponentManager.Object,
                messageBrokerConsumerMockObject,
                registrationOptions,
                loggerMock.Object);

            var cancellationTokenSource = new CancellationTokenSource();
            var listenTask = submittedJobConfigListenerService.Listen(cancellationTokenSource.Token);


            var testJobConfigUpdateNotification = new
            {
                Analysers = new string[] { "Analyser1", "Analyser2" },
                Networks = new string[] { "Network1" },
                TopicQuery = "topic1 and topic2"
            };

            var requestJson = JsonConvert.SerializeObject(testJobConfigUpdateNotification);
            messageBrokerConsumerMockObject.OnRecieveAction(requestJson);

            Assert.AreEqual("testContext.testComponent.testMessageType",
                messageBrokerConsumerMockObject.ConsumeTopic);

            cancellationTokenSource.Cancel();
            listenTask.Wait();

            subscribedComponentManager.Verify(
            sjcp => sjcp.PushJobConfigUpdateAsync(
                It.Is((JobConfigUpdateNotification val) =>
                   val.Analysers.Contains("Analyser1")
                       && val.Analysers.Contains("Analyser2")
                   && val.Networks.Contains("Network1")
                    && val.TopicQuery == "topic1 and topic2")),
                    Times.Once());

            subscribedComponentManager.VerifyNoOtherCalls();

        }

        [TestMethod]
        public async Task SubmittedJobConfigIntegrationTest()
        {
            // Arrange
            var componentRegistry = new ComponentRegistry();

            Assert.IsTrue(componentRegistry.AddOrUpdate(
                new ComponentRegistrationModel(
                    "analyser_1",
                    "j.c.a_1",
                    "Analyser")));

            Assert.IsTrue(componentRegistry.AddOrUpdate(
                new ComponentRegistrationModel(
                    "analyser_2",
                    "j.c.a_2",
                    "Analyser")));
            Assert.IsTrue(componentRegistry.AddOrUpdate(
                new ComponentRegistrationModel(
                    "network_1",
                    "j.c.n_1",
                    "Network")));
            Assert.IsTrue(componentRegistry.AddOrUpdate(
                new ComponentRegistrationModel(
                    "network_2",
                    "j.c.n_2",
                    "Network")));
            
            var messageBrokerProducerMock = new Mock<IMessageBrokerProducer>();
            var componentConfigNotifierLoggerMock = new Mock<ILogger<ComponentConfigUpdateNotifier>>();
            var componentConfigNotifier = new ComponentConfigUpdateNotifier(
                messageBrokerProducerMock.Object,
                componentConfigNotifierLoggerMock.Object
                );


            var subscribedCompnentLogger = new Mock<ILogger<SubscribedComponentManager>>();

            var subscribedComponentManager = new SubscribedComponentManager(
                componentRegistry,
                componentConfigNotifier,
                subscribedCompnentLogger.Object
                    );

            var notification = new JobConfigUpdateNotification(
                new List<string>() {"analyser_1", "analyser_2"},
                new List<string>() {"network_1", "network_2"},
                "Topic1 and Topic2");
            
            // Act
            await subscribedComponentManager.PushJobConfigUpdateAsync(notification);

            
            messageBrokerProducerMock.Verify(
                mbp=>mbp.ProduceAsync(It.Is<string>(cn=>cn=="j.c.a_1"),
                    It.IsAny<MessageBrokerMessage>()
                    ),Times.Once);

            messageBrokerProducerMock.Verify(
                mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.a_2"),
                    It.IsAny<MessageBrokerMessage>()
                ), Times.Once);

            messageBrokerProducerMock.Verify(
                mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.n_1"),
                    It.IsAny<MessageBrokerMessage>()
                ), Times.Once);

            messageBrokerProducerMock.Verify(
                mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.n_2"),
                    It.IsAny<MessageBrokerMessage>()
                ), Times.Once);

            messageBrokerProducerMock.VerifyNoOtherCalls();

        }
    }



}

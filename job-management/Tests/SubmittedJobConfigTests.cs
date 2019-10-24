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
        //[TestMethod]
        //public async Task SubmittedJobConfigIntegrationTest()
        //{
        //    // Arrange
        //    var componentRegistry = new ComponentRegistry();
            
        //    Assert.IsTrue(componentRegistry.AddOrUpdate(
        //        new ComponentRegistrationModel(
        //            "analyser_1",
        //            "j.c.a_1",
        //            "i.c.a_1",
        //            "DATA_ANALYSER",
        //            new Dictionary<string, string>())));

        //    Assert.IsTrue(componentRegistry.AddOrUpdate(
        //        new ComponentRegistrationModel(
        //            "analyser_2",
        //            "j.c.a_2",
        //            "i.c.a_2",
        //            "DATA_ANALYSER",
        //            new Dictionary<string, string>())));
        //    Assert.IsTrue(componentRegistry.AddOrUpdate(
        //        new ComponentRegistrationModel(
        //            "network_1",
        //            "j.c.n_1",
        //            "i.c.n_1",
        //            "DATA_ACQUIRER",
        //            new Dictionary<string, string>())));
        //    Assert.IsTrue(componentRegistry.AddOrUpdate(
        //        new ComponentRegistrationModel(
        //            "network_2",
        //            "j.c.n_2",
        //            "i.c.n_2",
        //            "DATA_ACQUIRER", new Dictionary<string, string>())));

        //    Assert.IsTrue(componentRegistry.AddOrUpdate(
        //        new ComponentRegistrationModel(
        //            "storage",
        //            "j.c.s_1",
        //            "i.c.s_1",
        //            "Storage", 
        //            new Dictionary<string, string>())));

        //    var messageBrokerProducerMock = new Mock<IMessageBrokerProducer>();
        //    var componentConfigNotifierLoggerMock = new Mock<ILogger<ComponentConfigUpdateNotifier>>();
        //    var componentConfigNotifier = new ComponentConfigUpdateNotifier(
        //        messageBrokerProducerMock.Object,
        //        componentConfigNotifierLoggerMock.Object
        //        );


        //    var subscribedCompnentLogger = new Mock<ILogger<SubscribedComponentManager>>();
            
        //    var subscribedComponentManager = new SubscribedComponentManager(
        //        componentRegistry,
        //        componentConfigNotifier,
        //        subscribedCompnentLogger.Object);
        //    var jobId = Guid.NewGuid();
        //    var notification = new JobConfigUpdateNotification(
        //        jobId,
        //        new List<string>() { "analyser_1", "analyser_2" },
        //        new List<string>() { "network_1", "network_2" },
        //        "Topic1 and Topic2");

        //    // Act
        //    await subscribedComponentManager.PushJobConfigUpdateAsync(notification);
            
        //    messageBrokerProducerMock.Verify(
        //        mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.a_1"),
        //            It.IsAny<MessageBrokerMessage>()
        //            ), Times.Once);

        //    messageBrokerProducerMock.Verify(
        //        mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.a_2"),
        //            It.IsAny<MessageBrokerMessage>()
        //        ), Times.Once);

        //    messageBrokerProducerMock.Verify(
        //        mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.n_1"),
        //            It.IsAny<MessageBrokerMessage>()
        //        ), Times.Once);

        //    messageBrokerProducerMock.Verify(
        //        mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.n_2"),
        //            It.IsAny<MessageBrokerMessage>()
        //        ), Times.Once);

        //    messageBrokerProducerMock.Verify(
        //        mbp => mbp.ProduceAsync(It.Is<string>(cn => cn == "j.c.s_1"),
        //            It.IsAny<MessageBrokerMessage>()
        //        ), Times.Once);


        //    messageBrokerProducerMock.VerifyNoOtherCalls();

        //}
    }



}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract;
using Domain.JobConfiguration;
using Domain.JobManagement;
using Infrastructure.DataGenerator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public async Task RandomDataAcquisitionIntegrationTest()
        {
            // TODO split test to multiple unit test that are not dependent upon time
            var randomGeneratorOptions = Options.Create(
                new RandomGeneratorOptions
                {
                    DownloadDelay = TimeSpan.FromSeconds(2),
                    Seed = 123
                });
            var dataAcquirer = new RandomDataGeneratorAcquirer(randomGeneratorOptions);
            var messageBrokerMock = new Mock<IMessageBrokerProducer>();
            var loggerMock = new Mock<ILogger<JobManager>>();
            var jobManager = new JobManager(
                dataAcquirer,
                messageBrokerMock.Object,
                loggerMock.Object);

            var guid = Guid.Parse("2b74e488-fb6b-4135-bf43-8cee811d140c");

            var dataAcquirerJobConfig = new DataAcquirerJobConfig
            {
                JobId = guid,
                Attributes = new Dictionary<string, string>()
                {
                    {"TopicQuery","Foo-Bar"}
                },

                OutputMessageBrokerChannels = new string[]{"test.channel.name"}
            };

            await jobManager.StartDownloadingAsync(
                dataAcquirerJobConfig);


            await Task.Delay(TimeSpan.FromSeconds(2));

            messageBrokerMock.Verify(
                r => r.ProduceAsync(It.IsAny<string>(), It.IsAny<MessageBrokerMessage>()),
                Times.Exactly(100));

        }
    }
}

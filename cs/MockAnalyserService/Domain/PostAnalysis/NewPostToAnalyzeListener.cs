using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Analyser;
using Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Domain.PostAnalysis
{
    public class NewPostToAnalyzeListener
    {
        private readonly IMessageBrokerConsumer _messageBrokerConsumer;
        private readonly IMessageBrokerProducer _producer;
        private readonly IAnalyser _analyser;
        private readonly IJobManager _jobManager;
        private readonly ILogger<NewPostToAnalyzeListener> _logger;
        private readonly string _inputChannelName;
        private readonly string _componentId;
        public bool ConnectionEstablished { get; private set; } = false;


        public NewPostToAnalyzeListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IMessageBrokerProducer producer,
            IAnalyser analyser,
            IJobManager jobManager,
            IOptions<ComponentOptions> componentOptionsAccessor,
            ILogger<NewPostToAnalyzeListener> logger)
        {
            if (string.IsNullOrEmpty(componentOptionsAccessor.Value.InputChannelName))
            {
                throw new ArgumentException("Argument must be valid channel name",
                    nameof(componentOptionsAccessor.Value.InputChannelName));
            }

            _messageBrokerConsumer = messageBrokerConsumer;
            _producer = producer;
            _analyser = analyser;
            _jobManager = jobManager;
            _logger = logger;

            _inputChannelName = componentOptionsAccessor.Value.InputChannelName;
            _componentId = componentOptionsAccessor.Value.ComponentId;
        }


        public Task ListenAsync(CancellationToken token)
        {
            return _messageBrokerConsumer.ConsumeAsync(
                _inputChannelName,
                ProcessNewPostAsync,
                token);
        }

        private async Task ProcessNewPostAsync(string postJson)
        {
            _logger.LogInformation("ROJ: {roj}",postJson);
            string outputChannelName = null;
            string sendObjectJson = null;
            try

            {


                var post = JsonConvert.DeserializeObject<UniPost>(postJson);
                var analysis = await _analyser.AnalyzePost(post);
                var sendObject = AnalysisResponse.FromData(_componentId, post, analysis);
                sendObjectJson = JsonConvert.SerializeObject(sendObject);

                outputChannelName = "job_management.component_data_analyzed_input.storage_db";
            }
            catch (Exception e)
            {
                _logger.LogError("Analysing error {err}",e.Message);
                return;
            }

            _logger.LogInformation("SOJ: {soj}",sendObjectJson);
            var messageBrokerMessage = new MessageBrokerMessage(
                "analyzed-post",
                sendObjectJson);
            await _producer.ProduceAsync(outputChannelName, messageBrokerMessage);
        }
    }
}
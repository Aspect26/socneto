using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Analyser;
using Domain.Model;
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
        private readonly string _inputChannelName;
        private readonly string _componentId;
        public bool ConnectionEstablished { get; private set; } = false;


        public NewPostToAnalyzeListener(
            IMessageBrokerConsumer messageBrokerConsumer,
            IMessageBrokerProducer producer,
            IAnalyser analyser,
            IJobManager jobManager,
            IOptions<ComponentOptions> componentOptionsAccessor)
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

            _inputChannelName = componentOptionsAccessor.Value.InputChannelName;
            _componentId = componentOptionsAccessor.Value.ComponentId;
        }

        public void OnConnectionEstablished()
        {
            ConnectionEstablished = true;
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
            var post = JsonConvert.DeserializeObject<UniPost>(postJson);
            var analysis = await _analyser.AnalyzePost(post);
            var sendObject = AnalysisResponse.FromData(_componentId, post, analysis);
            var sendObjectJson = JsonConvert.SerializeObject(sendObject);
            
            var messageBrokerMessage = new MessageBrokerMessage(
                "analyzed-post",
                sendObjectJson);

            var outputChannelName= _jobManager.GetJobConfigOutput(post.JobId);
            
            await _producer.ProduceAsync(outputChannelName, messageBrokerMessage);
        }
    }
}
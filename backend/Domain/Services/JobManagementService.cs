using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Socneto.Domain.EventTracking;
using Socneto.Domain.Models;
using Socneto.Domain.Models.JMS.Response;

namespace Socneto.Domain.Services
{
    public class JobManagementService : IJobManagementService
    {
        
        private readonly HttpService<JobManagementService> _httpService;
        private readonly DefaultAcquirersCredentialsOptions _defaultAcquirersCredentials;
        
        public JobManagementService(IEventTracker<JobManagementService> eventTracker, 
            IOptions<DefaultAcquirersCredentialsOptions> defaultAcquirersCredentialsOptionsObject, 
            IOptions<JMSOptions> jmsOptionsObject)
        {
            if (string.IsNullOrEmpty(jmsOptionsObject.Value.ServerAddress))
                throw new ArgumentNullException(nameof(jmsOptionsObject.Value.ServerAddress));
            
            if (defaultAcquirersCredentialsOptionsObject.Value?.Twitter == null)
                throw new ArgumentNullException(nameof(defaultAcquirersCredentialsOptionsObject.Value.Twitter));
            
            if (defaultAcquirersCredentialsOptionsObject.Value?.Reddit == null)
                throw new ArgumentNullException(nameof(defaultAcquirersCredentialsOptionsObject.Value.Reddit));
            
            _defaultAcquirersCredentials = defaultAcquirersCredentialsOptionsObject.Value;
            
            var host = jmsOptionsObject.Value.ServerAddress;
            _httpService = new HttpService<JobManagementService>(host, eventTracker);
        }
        
        private Dictionary<string, string> DefaultTwitterCredentials => new Dictionary<string, string>
        {
            ["ApiKey"] = _defaultAcquirersCredentials.Twitter.ApiKey,
            ["ApiSecretKey"] = _defaultAcquirersCredentials.Twitter.ApiSecretKey,
            ["AccessToken"] = _defaultAcquirersCredentials.Twitter.AccessToken,
            ["AccessTokenSecret"] = _defaultAcquirersCredentials.Twitter.AccessTokenSecret
        };
        
        private Dictionary<string, string> DefaultRedditCredentials => new Dictionary<string, string>
        {
            ["appId"] = _defaultAcquirersCredentials.Reddit.AppId,
            ["appSecret"] = _defaultAcquirersCredentials.Reddit.AppSecret,
            ["refreshToken"] = _defaultAcquirersCredentials.Reddit.RefreshToken
        };

        public async Task<bool> IsComponentRunning()
        {
            try
            {
                var hello = await _httpService.Get<JmsHelloResponse>("api/test/say-hello");
                return hello.Message == "hello";
            }
            catch (ServiceUnavailableException)
            {
                return false;
            }
        }

        public async Task<JobStatus> SubmitJob(JobSubmit jobSubmit, Dictionary<string, Dictionary<string, string>> originalAttributes)
        {
            var attributes = new JObject();
            foreach (var selectedAcquirerId in jobSubmit.SelectedAcquirersIdentifiers)
            {
                var acquirerAttributes = originalAttributes.ContainsKey(selectedAcquirerId)
                    ? originalAttributes[selectedAcquirerId]
                    : new Dictionary<string, string>();

                AddDefaultTwitterCredentialsIfNotSet(acquirerAttributes);
                AddDefaultRedditCredentialsIfNotSet(acquirerAttributes);
                
                attributes.Add(selectedAcquirerId, new JObject(
                    acquirerAttributes.ToList()
                        .Where(attribute => !string.IsNullOrEmpty(attribute.Key))
                        .Select(attribute => new JProperty(attribute.Key, attribute.Value)).ToList())
                );
            }


            jobSubmit.Attributes = attributes;
            return await _httpService.Post<JobStatus>($"api/job/submit", jobSubmit);
        }

        public async Task<JobStatus> StopJob(Guid jobId)
        {
            return await _httpService.Post<JobStatus>($"api/job/stop/{jobId}");
        }

        private void AddDefaultTwitterCredentialsIfNotSet(Dictionary<string, string> credentials)
        {
            AddValuesIfAllMissing(credentials, DefaultTwitterCredentials);
        }
        
        private void AddDefaultRedditCredentialsIfNotSet(Dictionary<string, string> credentials)
        {
            AddValuesIfAllMissing(credentials, DefaultRedditCredentials);
        }

        private void AddValuesIfAllMissing(Dictionary<string, string> into, Dictionary<string, string> from)
        {
            if (into.Keys.Count == into.Keys.Except(from.Keys).Count())
            {
                from.ToList().ForEach(x => into[x.Key] = x.Value);
            }
        }
    }
}
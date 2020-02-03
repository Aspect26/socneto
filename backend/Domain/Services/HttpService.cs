using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Socneto.Domain.EventTracking;

namespace Socneto.Domain.Services
{
    public class HttpService<T>
    {
        private readonly string _host;
        private readonly HttpClient _client = new HttpClient();
        private readonly IEventTracker<T> _eventTracker;

        private string EventTrackerEventName => $"Backend HTTP request on to {_host}";

        public HttpService(string host, IEventTracker<T> eventTracker)
        {
            _host = host;
            _eventTracker = eventTracker;
        }
        
        public async Task<TResult> Get<TResult>(string path)
        {
            var fullPath = GetFullPath(path);
            _eventTracker.TrackInfo(EventTrackerEventName,$"GET /{path}");

            try
            {
                var response = await _client.GetAsync(fullPath);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<TResult>();
            }
            catch (HttpRequestException e)
            {
                throw WrapHttpException(e);
            }
        }
        
        public async Task<TResult> Post<TResult>(string path, object data = null)
        {
            var fullPath = GetFullPath(path);
            var content = CreateHttpContent(data ?? new object());
            _eventTracker.TrackInfo(EventTrackerEventName,$"POST /{path} {content}");

            try
            {
                var response = await _client.PostAsync(fullPath, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<TResult>();
            }
            catch (HttpRequestException e)
            {
                throw WrapHttpException(e);
            }
        }
        
        public async Task<TResult> Put<TResult>(string path, object data)
        {
            var fullPath = GetFullPath(path);
            var content = CreateHttpContent(data);
            _eventTracker.TrackInfo(EventTrackerEventName,$"PUT /{path} {content}");

            try
            {
                var response = await _client.PutAsync(fullPath, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsAsync<TResult>();
            }
            catch (HttpRequestException e)
            {
                throw WrapHttpException(e);
            }
        }

        private Exception WrapHttpException(HttpRequestException exception)
        {
            if (exception.InnerException is SocketException)
            {
                return new ServiceUnavailableException(_host, exception);
            }

            return exception;
        }
        
        private string GetFullPath(string path)
        {
            return $"{_host}/{path}";
        }
        
        private HttpContent CreateHttpContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

    }

    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string host, Exception innerException) : base($"Service '{host}' unavailable", innerException)
        {
            
        }
    }
}
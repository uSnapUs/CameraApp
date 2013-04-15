using System.Collections.Generic;
using System.Net;
using Camera.Exceptions;
using Camera.Model;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;

namespace Camera.Helpers
{
    public class Server : IServer
    {
        readonly ILogger _logger;
        Credientials _currentCredentials;

        public Server(ILogger logger)
        {
            _logger = logger;
        }

        public string BaseUrl = "http://api.isnap.us/";

        public DeviceRegistration RegisterDevice(DeviceRegistration deviceRegistration)
        {
            
            var client = GetClient();
            if (_currentCredentials != null&& _currentCredentials.Guid != deviceRegistration.Guid)
            {
                client.Authenticator = null;
            }
            var request = RestClientFactory.CreateRestRequest("devices", Method.POST);
            request.JsonSerializer = new JsonDotNetSerializer();
            request.RequestFormat = DataFormat.Json;
            
            request.AddBody(deviceRegistration);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Post<DeviceRegistration>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            var exception = new ApiException(response.Content);
            _logger.Exception(exception);
            throw exception;
        }
        IRestClient GetClient()
        {

            var client = RestClientFactory.CreateClient(BaseUrl);
            if (_currentCredentials != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(_currentCredentials.Guid, _currentCredentials.Token);
            }
            return client;
        }
        IRestClientFactory _restClientFactory;
        public IRestClientFactory RestClientFactory
        {
            get { return _restClientFactory ?? (_restClientFactory = new RestClientFactory()); }
            set
            {
                _restClientFactory = value;
            }
        }
        public void SetDeviceCredentials(string guid, string token)
        {
            _currentCredentials = new Credientials { Guid = guid, Token = token };
        }

        public Event CreateEvent(Event eventToCreate)
        {
            var client = GetClient();
           
            var request = RestClientFactory.CreateRestRequest("events", Method.POST);
            request.JsonSerializer = new JsonDotNetSerializer();
            request.RequestFormat = DataFormat.Json;

            request.AddBody(eventToCreate);
            var response = client.Post<Event>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            var exception = new ApiException(response.Content);
            _logger.Exception(exception);
            throw exception;
        }

        public Event FindEvent(string code)
        {
            var client = GetClient();
            var request = RestClientFactory.CreateRestRequest("event/" + code, Method.GET);
            request.JsonSerializer = new JsonDotNetSerializer();
            request.RequestFormat = DataFormat.Json;
            var response = client.Get<Event>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            var exception = new ApiException(response.Content);
            _logger.Exception(exception);
            throw exception;
        }

        public Event[] FindEventsCloseTo(Coordinate coordinate)
        {
            var client = GetClient();
            var request = RestClientFactory.CreateRestRequest("events/by_location?longitude="+coordinate.Longitude+"&latitude="+coordinate.Latitude, Method.GET);
            
            request.JsonSerializer = new JsonDotNetSerializer();
            request.RequestFormat = DataFormat.Json;
            var response = client.Get<List<Event>>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.ToArray();
            }
            var exception = new ApiException(response.Content);
            _logger.Exception(exception);
            throw exception;
        }
    }

    public class JsonDotNetSerializer : ISerializer
    {
        string _contentType;

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public string ContentType { get { return _contentType??"application/json"; } set { _contentType = value; } }
    }

    public class Credientials
    {
        public string Guid { get; set; }

        public string Token { get; set; }
    }
    
}
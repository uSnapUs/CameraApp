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
            if (_currentCredentials != null && _currentCredentials.Guid == deviceRegistration.Guid)
            {
                client.Authenticator = new HttpBasicAuthenticator(_currentCredentials.Guid,_currentCredentials.Token);
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

            return RestClientFactory.CreateClient(BaseUrl);
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
using System.Net;
using Camera.Exceptions;
using Camera.Model;

using RestSharp;
using TinyMessenger;

namespace Camera.Helpers
{
    public class Server : IServer
    {
        readonly ITinyMessengerHub _messengerHub;
        readonly ILogger _logger;

        public Server(ITinyMessengerHub messengerHub, ILogger logger)
        {
            _messengerHub = messengerHub;
            _logger = logger;
        }

        public string BaseUrl = "http://api.stage.isnap.us/";

        public DeviceRegistration RegisterDevice(DeviceRegistration deviceRegistration)
        {
            var client = GetClient();
            var request = RestClientFactory.CreateRestRequest("device", Method.POST);
            
            request.RequestFormat = DataFormat.Json;
            
            request.AddBody(deviceRegistration.ToDto());
            request.AddHeader("Content-Type", "application/json");

            var response = client.Post<DeviceRegistrationDto>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data.ToModel();
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

       
    }

    
}
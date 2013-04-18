using System;
using System.Collections.Generic;
using System.Net;
using Camera.Exceptions;
using Camera.Model;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using TinyMessenger;

namespace Camera.Helpers
{
    public class Server : IServer
    {
        readonly ILogger _logger;
        readonly ITinyMessengerHub _tinyMessenger;
        Credientials _currentCredentials;

        public Server(ILogger logger, ITinyMessengerHub tinyMessenger)
        {
            _logger = logger;
            _tinyMessenger = tinyMessenger;
            _tinyMessenger.Subscribe<UploaderDoneMessage>(message =>
                {
                    var photoUploader = message.PhotoUploader;
                    if (_photoUploaders.Contains(photoUploader))
                    {
                        _photoUploaders.Remove(photoUploader);
                    }
                });
        }

        readonly List<PhotoUploader> _photoUploaders = new List<PhotoUploader>();
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
        public void PostPhoto(string code, string path,Guid photoIdentifier)
        {
            _photoUploaders.Add(new PhotoUploader(code, path, photoIdentifier, this));
        }

        

        public Event[] FindEventsByLocation(Coordinate coordinate)
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
        internal class PhotoUploader
        {
            WebClient _webClient;
            readonly ITinyMessengerHub _tinyMessenger;

            public PhotoUploader(string code, string path, Guid photoIdentifier, Server server)
            {
                this._tinyMessenger = server._tinyMessenger;
                _webClient = new WebClient
                {
                    BaseAddress = server.BaseUrl,
                    Credentials = new NetworkCredential(server._currentCredentials.Guid, server._currentCredentials.Token),
                };
                _webClient.UploadProgressChanged += WebClientOnUploadProgressChanged;
                _webClient.UploadFileCompleted += WebClientOnUploadFileCompleted;
                _webClient.UploadFileAsync(new Uri(server.BaseUrl + "/event/" + code + "/photos", UriKind.Absolute), "POST", path);
            }

            void WebClientOnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
            {
                
            }

            void WebClientOnUploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
            {
                _webClient.UploadProgressChanged -= WebClientOnUploadProgressChanged;
                _webClient.UploadFileCompleted -= WebClientOnUploadFileCompleted;
                _webClient.Dispose();
                _webClient = null;
                _tinyMessenger.PublishAsync(new UploaderDoneMessage{Sender = this,PhotoUploader = this});
            }
        }
    }

    internal class UploaderDoneMessage : ITinyMessage
    {
        public Server.PhotoUploader PhotoUploader { get; set; }
        public object Sender { get; set; }
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
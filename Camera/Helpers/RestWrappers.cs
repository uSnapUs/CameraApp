using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace Camera.Helpers
{
    public class RestClientFactory : IRestClientFactory
    {

        public IRestClient CreateClient(string baseUrl)
        {
            var rc = new RestClient(baseUrl) { };
            rc.AddHandler("application/json",new JsonDotNetDeserializer());
            if (Proxy != null)
                rc.Proxy = Proxy;
            return rc;
        }

        public IWebProxy Proxy { get; set; }

        public IRestRequest CreateRestRequest(string path, Method method)
        {
            return new RestRequest(path, method);
        }
    }

    public class JsonDotNetDeserializer : IDeserializer
    {
        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}
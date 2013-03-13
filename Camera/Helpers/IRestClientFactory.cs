using RestSharp;

namespace Camera.Helpers
{
    public interface IRestClientFactory
    {
        IRestClient CreateClient(string baseUrl);
        IRestRequest CreateRestRequest(string path, Method method);
    }
}
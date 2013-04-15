using Camera.Model;

namespace Camera.Helpers
{
    public interface IServer
    {
        DeviceRegistration RegisterDevice(DeviceRegistration deviceRegistration);
        IRestClientFactory RestClientFactory { get; }
        void SetDeviceCredentials(string guid, string token);
        Event CreateEvent(Event eventToCreate);
        Event FindEvent(string eventCode);
        Event[] FindEventsByLocation(Coordinate coordinate);
    }
}
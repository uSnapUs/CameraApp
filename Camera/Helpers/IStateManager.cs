using System;
using Camera.Model;
using TinyMessenger;

namespace Camera.Helpers
{
    public interface IStateManager : IDisposable
    {
        event EventHandler<EventArgs> Authenticated;
        IServer Server { get; set; }
        DeviceRegistration CurrentDeviceRegistration { get; }
        string DeviceName { set; }
        bool IsAuthenticated { get; }
        ILocationCoder LocationCoder { get; set; }
        Coordinate? CurrentLocation { get; set; }
        ILocationManager LocationManager { get; set; }
        ITinyMessengerHub MessageHub { get; }
        void UpdateDeviceRegistration(string name, string email, string facebookId);
        void InitiateFacebookLogin();

        Event CreateEvent(Event eventToCreate);
        void UploadPhoto(Event currentEvent,string photoPath);
        Photo[] GetEventPhotos(Event ev);
        void StartUpdatingPhotosForEvent(Event ev);
        void StopUpdatingPhotosForEvent();
    }
}
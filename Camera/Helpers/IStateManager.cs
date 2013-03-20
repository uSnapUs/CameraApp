using System;
using Camera.Model;

namespace Camera.Helpers
{
    public interface IStateManager : IDisposable
    {
        IServer Server { get; }
        DeviceRegistration CurrentDeviceRegistration { get; }
        string DeviceName { set; }
        User CurrentUser { get; set; }
        void UpdateDeviceRegistration(string name, string email, string facebookId);
        void InitiateFacebookLogin();
    }
}
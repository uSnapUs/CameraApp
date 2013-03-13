using System;
using Camera.Model;

namespace Camera.Helpers
{
    public interface IStateManager : IDisposable
    {
        IServer Server { get; }
        DeviceRegistration CurrentDeviceRegistration { get; }
        string DeviceName { set; }
        void UpdateDeviceRegistration(string name, string email, string facebookId);
    }
}
﻿using System;
using Camera.Model;

namespace Camera.Helpers
{
    public interface IStateManager : IDisposable
    {
        event EventHandler<EventArgs> Authenticated;
        IServer Server { get; }
        DeviceRegistration CurrentDeviceRegistration { get; }
        string DeviceName { set; }
        bool IsAuthenticated { get; }
        void UpdateDeviceRegistration(string name, string email, string facebookId);
        void InitiateFacebookLogin();

    }
}
using System;

namespace Camera.Views.Interfaces
{

    public interface ILandingPageView
    {
        event EventHandler<EventArgs> FindButtonPressed;
        event EventHandler<EventArgs> CreateButtonPressed;
        event EventHandler<EventArgs> MyEventsButtonPressed;
        event EventHandler<EventArgs> BackButtonPressed;
        event EventHandler<EventArgs> FindNearbyButtonPressed;
        event EventHandler<EventArgs> JoinButtonPressed;

    }
}
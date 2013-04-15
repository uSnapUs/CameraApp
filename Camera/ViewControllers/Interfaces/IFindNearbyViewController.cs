using System;
using Camera.Helpers;
using Camera.Model;

namespace Camera.ViewControllers.Interfaces
{
    public interface IFindNearbyViewController:IBaseViewController
    {
        event EventHandler<EventArgs> BackButtonPressed;
        void GoToLandingView();
        Coordinate GetMapLocation();
        void ShowEventAnnotations(Event[] eventsNearby);
    }
}
using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface IFindNearbyViewController:IBaseViewController
    {
        event EventHandler<EventArgs> BackButtonPressed;
        void GoToLandingView();
    }
}
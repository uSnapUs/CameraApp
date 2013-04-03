using System;
using Camera.Helpers;

namespace Camera.ViewControllers.Interfaces
{
    public interface ICreateEventViewController:IBaseViewController
    {
        void PresentLandingView();
        event EventHandler<EventArgs> BackPressed;
        event EventHandler<EventArgs> LocationLookupShown;
        void SetLocation(Coordinate coordinate);
    }
}
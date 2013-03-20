using System;

namespace Camera.ViewControllers.Interfaces
{
    public interface IEventDashboardViewController:IBaseViewController
    {
        event EventHandler<EventArgs> BackButtonPressed;
        event EventHandler<EventArgs> CameraButtonPressed;
        void PresentLandingView();
        void PresentImagePickerView();
    }
}
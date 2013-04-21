using System;
using Camera.Model;

namespace Camera.ViewControllers.Interfaces
{
    public interface IEventDashboardViewController:IBaseViewController
    {
        event EventHandler<EventArgs> BackButtonPressed;
        event EventHandler<EventArgs> CameraButtonPressed;
        event EventHandler<ImageEventArgs> ImageSelected;
        Event Event { get; }
        Photo[] Photos { get; set; }
        void PresentLandingView();
        void PresentImagePickerView();
        void ProgressUploadMessage(float percentageDone);
        void StartUploadMessage();
        void ClearUploadMessage(bool uploadOk);
        void ShowUpdatingMessage();
    }

    public class ImageEventArgs : EventArgs
    {
        public Byte[] Image { get; set; }
    }
}
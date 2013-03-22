using System;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    [Register("EventDashboardViewController")]
    public sealed class EventDashboardViewController : BaseViewController,IEventDashboardViewController
    {
        EventDashboardView _eventDashboardView;
        EventDashboardViewControllerSupervisor _supervisor;

        public EventDashboardViewController()
        {
            UIApplication.SharedApplication.SetStatusBarHidden(false,true);
            _eventDashboardView = new EventDashboardView();
            _eventDashboardView.BackButtonPressed += EventDashboardBackButtonPressed;
            _eventDashboardView.CameraButtonPressed += EventDashboardViewOnCameraButtonPressed;
            _supervisor = new EventDashboardViewControllerSupervisor(this);
            View = _eventDashboardView;

        }

        void EventDashboardViewOnCameraButtonPressed(object sender, EventArgs eventArgs)
        {
            OnCameraButtonPressed();
        }

        void EventDashboardBackButtonPressed(object sender, EventArgs eventArgs)
        {
           OnBackButtonPressed();
        }


        protected override void EnsureSupervised()
        {
            if (_supervisor == null)
            {
                _supervisor = new EventDashboardViewControllerSupervisor(this);
            }
        }

        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> CameraButtonPressed;

        void OnCameraButtonPressed()
        {
            EventHandler<EventArgs> handler = CameraButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void OnBackButtonPressed()
        {
            EventHandler<EventArgs> handler = BackButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void PresentLandingView()
        {
            var landingPageViewController = new LandingPageViewController();
            PresentViewController(landingPageViewController,true,Dispose);

        }

        public void PresentImagePickerView()
        {
            var imagePickerViewController = new ImagePickerViewController();
            imagePickerViewController.ImageCaptured += ImagePickerViewControllerOnImageCaptured;
            imagePickerViewController.Cancel += ImagePickerViewControllerOnCancel;
            PresentViewController(imagePickerViewController,true,null);
        }

        void ImagePickerViewControllerOnCancel(object sender, EventArgs eventArgs)
        {
            DismissViewController(true,null);
            var imagePickerViewController = sender as ImagePickerViewController;
            if (imagePickerViewController != null)
            {
                imagePickerViewController.ImageCaptured -= ImagePickerViewControllerOnImageCaptured;
                imagePickerViewController.Cancel -= ImagePickerViewControllerOnCancel;
                imagePickerViewController.Dispose();
            }
        }

        void ImagePickerViewControllerOnImageCaptured(object sender, ImageCaptureEvent imageCaptureEvent)
        {
            DismissViewController(true, null);
            var imagePickerViewController = sender as ImagePickerViewController;
            if (imagePickerViewController != null)
            {
                imagePickerViewController.ImageCaptured -= ImagePickerViewControllerOnImageCaptured;
                imagePickerViewController.Cancel -= ImagePickerViewControllerOnCancel;
                imagePickerViewController.Dispose();
            }
        }


        protected override void Dispose(bool disposing)
        {
            _eventDashboardView.BackButtonPressed -= EventDashboardBackButtonPressed;
            _eventDashboardView.CameraButtonPressed -= EventDashboardViewOnCameraButtonPressed;
            _eventDashboardView = null;
            _supervisor = null;
            base.Dispose(disposing);
            
        }

        
    }
}
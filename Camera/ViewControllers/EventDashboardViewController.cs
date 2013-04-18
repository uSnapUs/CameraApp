using System;
using Camera.Model;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.AssetsLibrary;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    [Register("EventDashboardViewController")]
    public sealed class EventDashboardViewController : BaseViewController,IEventDashboardViewController
    {
        EventDashboardView _eventDashboardView;
        EventDashboardViewControllerSupervisor _supervisor;

        public EventDashboardViewController(Event serverEvent)
        {
            UIApplication.SharedApplication.SetStatusBarHidden(false,true);
            _eventDashboardView = new EventDashboardView();
            _eventDashboardView.BackButtonPressed += EventDashboardBackButtonPressed;
            _eventDashboardView.CameraButtonPressed += EventDashboardViewOnCameraButtonPressed;
            _supervisor = new EventDashboardViewControllerSupervisor(this);
            if (serverEvent != null)
            {
                _eventDashboardView.Title = serverEvent.Name;
            }
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
         
            if (NSThread.IsMain) {
             Console.WriteLine("On Main Thread");
            }
            else
            {
                Console.WriteLine("Not on Main Thread");
            }
        
            Console.WriteLine();
            var imagePickerViewController = sender as ImagePickerViewController;
            if (imagePickerViewController != null)
            {
                imagePickerViewController.ImageCaptured -= ImagePickerViewControllerOnImageCaptured;
                imagePickerViewController.Cancel -= ImagePickerViewControllerOnCancel;
                DismissViewController(true,imagePickerViewController.Dispose);
            }
            var library = new ALAssetsLibrary();
            var alaOrentation = ALAssetOrientation.Down;
            switch (imageCaptureEvent.Image.Orientation)
            {
                case UIImageOrientation.Down:
                    alaOrentation = ALAssetOrientation.Down;
                    break;
                case UIImageOrientation.DownMirrored:
                    alaOrentation = ALAssetOrientation.DownMirrored;
                    break;
                case UIImageOrientation.Left:
                    alaOrentation = ALAssetOrientation.Left;
                    break;
                case UIImageOrientation.LeftMirrored:
                    alaOrentation = ALAssetOrientation.LeftMirrored;
                    break;
                case UIImageOrientation.Right:
                    alaOrentation = ALAssetOrientation.Right;
                    break;
                case UIImageOrientation.RightMirrored:
                    alaOrentation = ALAssetOrientation.RightMirrored;
                    break;
                case UIImageOrientation.Up:
                    alaOrentation = ALAssetOrientation.Up;
                    break;
                case UIImageOrientation.UpMirrored:
                    alaOrentation = ALAssetOrientation.UpMirrored;
                    break;
            }
            library.WriteImageToSavedPhotosAlbum(imageCaptureEvent.Image.CGImage, alaOrentation, (url, error) =>
                {
                    if (error != null)
                        return;
                    AddAssetToAlbum(url);
                });

        }

        void AddAssetToAlbum(NSUrl result)
        {
            var library = new ALAssetsLibrary();
            bool albumWasFound = false;
            
            library.Enumerate(ALAssetsGroupType.Album, (ALAssetsGroup group, ref bool stop) =>
                {
                   
                    if (group!=null&&group.Name == "uSnap.us")
                    {
                       library.AssetForUrl(result, asset => 
                               group.AddAsset(asset),
                               error => { });
                        albumWasFound = true;
                        stop = true;
                        return;
                    }
                    if (group == null && !albumWasFound)
                    {
                        library.AddAssetsGroupAlbum("uSnap.us", 
                            albumGroup => library.AssetForUrl(result, asset =>albumGroup.AddAsset(asset)
                                , error => { }),
                                error => { });
                    }


                }, error =>
                    {
                        
                    });
           
            
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
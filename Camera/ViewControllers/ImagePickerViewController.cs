using System;
using System.Collections.Generic;
using System.Drawing;
using Camera.Views;
using GPUImage;
using MonoTouch.AVFoundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    public sealed class ImagePickerViewController:BaseViewController
    {

        ImagePickerView _imagePickerView;
        bool _hasBlur;

        bool _isStatic;

        GPUImageStillCamera _stillCamera;
        GPUImageCropFilter _cropFilter;
        GPUImageFilter _filter;
        GPUImageGaussianSelectiveBlurFilter _blurFilter;
        GPUImagePicture _staticPicture;
        UIImageOrientation _staticPictureOriginalOrientation;
        readonly List<UIButton> _filterButtons = new List<UIButton> ();


        int _selectedFilter;

        public ImagePickerViewController ()
        {
            _imagePickerView =new ImagePickerView();
            SetupEvents ();
            View = _imagePickerView;
           // ViewDidLoad ();
        }
        protected override void Dispose (bool disposing)
        {
            UnwireEvents ();
            _filter = null;
            _staticPicture = null;
            _stillCamera = null;
            
            _imagePickerView = null;

            base.Dispose (disposing);

        }

        void UnwireEvents ()
        {
            
            _imagePickerView.LibraryButtonPressed -= SwitchToLibrary;
            _imagePickerView.FlipButtonPressed -= FlipCameraView;
            _imagePickerView.ShutterButtonPressed -= TakePhoto;
            _imagePickerView.RetakeButtonPressed -= RetakePhoto;			
            _imagePickerView.CloseButtonPressed -= CloseView;
            _imagePickerView.FilterButtonPressed -= ToggleFilters;
            _imagePickerView.FlashButtonPressed -= ToggleFlash;
            _imagePickerView.ImageTapped -= FocusCamera;
            _imagePickerView.ImagePinched -= HandlePinch;
            foreach (var filterButton in _filterButtons) {
                filterButton.TouchUpInside-=FilterSelected;
            }
        }

        void SetupEvents ()
        {
            _imagePickerView.LibraryButtonPressed += SwitchToLibrary;
            _imagePickerView.FlipButtonPressed += FlipCameraView;
            _imagePickerView.ShutterButtonPressed += TakePhoto;
            _imagePickerView.CloseButtonPressed += CloseView;
            _imagePickerView.RetakeButtonPressed += RetakePhoto;
            _imagePickerView.FilterButtonPressed += ToggleFilters;
            _imagePickerView.FlashButtonPressed += ToggleFlash;

            _imagePickerView.ImageTapped += FocusCamera;
            _imagePickerView.ImagePinched += HandlePinch;
        }

        public override void ViewWillAppear (bool animated)
        {
            UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide);
            base.ViewWillAppear (animated);
        }

        void CloseView (object sender, EventArgs e)
        {
            DismissViewController (true, Dispose);
        }

        protected override void EnsureSupervised()
        {
            
        }

        public override void ViewDidLoad ()
        {
            WantsFullScreenLayout = true;
            _staticPictureOriginalOrientation = UIImageOrientation.Up;
            _hasBlur = false;
            LoadFilters ();
            _cropFilter = new GPUImageCropFilter(new RectangleF(0f,0f,1f,0.75f));
            _filter = new GPUImageFilter ();

            SetupCamera();
        }
        public void OnPinch(UIPinchGestureRecognizer pinchGesture){
            

        }
        void RunOnMainQueueWithoutDeadlocking(NSAction action){
            if (NSThread.IsMain) {
                action.Invoke ();
            }else {
                DispatchQueue.MainQueue.DispatchSync(action);
            }
        }

        void LoadFilters ()
        {
            for (int i=0; i<10; i++) {
                var button = new UIButton(UIButtonType.Custom);
                button.SetBackgroundImage(UIImage.FromFile(String.Format("{0}.jpg",i+1)),UIControlState.Normal);
                button.Frame = new RectangleF(10+i*(60+10),5f,60f,60f);
                button.Layer.CornerRadius = 7f;
                var bi = UIBezierPath.FromRoundedRect(button.Bounds,UIRectCorner.AllCorners,new SizeF(7,7));
                var maskLayer = new CAShapeLayer {Frame = button.Bounds, Path = bi.CGPath};

                button.Layer.Mask = maskLayer;
                button.Layer.BorderWidth = 1;
                button.Layer.BorderColor = UIColor.Black.CGColor;

                button.TouchUpInside+=FilterSelected;
                button.Tag = i;
                if(i==0){
                    button.Selected = true;
                }
                _imagePickerView.FilterScrollView.AddSubview(button);
                _filterButtons.Add(button);
            }
            _imagePickerView.FilterScrollView.ContentSize = new SizeF (10 + 10 * (60 + 10), 75);
        }

        void PrepareFilter ()
        {
            Console.WriteLine ("Preparing filter");
            if (!UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera)) {
                _isStatic = true;
            }
            if (!_isStatic) {
                PrepareLiveFilter();
        } else {
                PrepareStaticFilter();
        }

        }

        void FilterSelected (object sender, EventArgs e)
        {
            foreach (var view in _imagePickerView.FilterScrollView.Subviews) {
                var button = (view as UIButton);
                if(button!=null){
                    button.Selected = false;
                }

            }
            var senderButton = sender as UIButton;
            if (senderButton != null) {
                senderButton.Selected = true;
                RemoveAllTargets();
                _selectedFilter = senderButton.Tag;
                SetFilter(_selectedFilter);
                PrepareFilter();
            }
        }

        void PrepareLiveFilter ()
        {
            _stillCamera.AddTarget(_cropFilter);
            _cropFilter.AddTarget(_filter);
            if (_hasBlur) {
                _filter.AddTarget(_blurFilter);
                _blurFilter.AddTarget(_imagePickerView.ImageView);
            } else {
                _filter.AddTarget (_imagePickerView.ImageView);
            }
        }

        void PrepareStaticFilter ()
        {
            if (_staticPicture == null) {
                NSTimer.CreateScheduledTimer (0.5,()=> SwitchToLibrary(this,null));
                return;
            }

            _staticPicture.AddTarget(_filter);
            if(_hasBlur){
                _filter.AddTarget(_blurFilter);
                _blurFilter.AddTarget(_imagePickerView.ImageView);
            }
            else{
                    _filter.AddTarget(_imagePickerView.ImageView);
            }
            GPUImageRotationMode imageViewRotationMode;
            switch (_staticPictureOriginalOrientation) {
            case UIImageOrientation.Left:
                imageViewRotationMode = GPUImageRotationMode.kGPUImageRotateLeft;
                break;
            case UIImageOrientation.Right:
                imageViewRotationMode = GPUImageRotationMode.kGPUImageRotateRight;
                break;
            case UIImageOrientation.Down:
                imageViewRotationMode = GPUImageRotationMode.kGPUImageRotate180;
                break;
            default:
                imageViewRotationMode = GPUImageRotationMode.kGPUImageNoRotation;
                break;

            }
            _imagePickerView.ImageView.SetInputRotationAtIndex (imageViewRotationMode, 0);
            _staticPicture.ProcessImage ();
        }

        void SetupCamera ()
        {
            if (UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera)) {
                DispatchQueue.MainQueue.DispatchAsync (() => {
                    _stillCamera = new GPUImageStillCamera (AVCaptureSession.PresetPhoto, AVCaptureDevicePosition.Back) {
                        OutputImageOrientation = UIInterfaceOrientation.Portrait
                    };
                                                                 RunOnMainQueueWithoutDeadlocking (() => {
                        _stillCamera.StartCameraCapture ();
                        _imagePickerView.FlashButton.Enabled = _stillCamera.InputCamera.TorchAvailable;
                        PrepareFilter ();
                    });
                });
            } else {
                RunOnMainQueueWithoutDeadlocking(PrepareFilter);
            }
        }
        void SetFilter(int index)
        {
            switch (index) {
            case 1:
                var contrastFilter = new GPUImageContrastFilter {Contrast = 1.75f};
                    _filter = contrastFilter;
                break;
            case 2: 
                _filter = new GPUImageToneCurveFilter ("crossprocess");    
                break;
            case 3: 
                _filter = new GPUImageToneCurveFilter (@"02");
                break;
            case 4: 
                _filter = new GPUGrayscaleContrastFilter ();
                break;
            case 5: 
                _filter = new GPUImageToneCurveFilter (@"17");
                break;
            case 6: 
                _filter = new GPUImageToneCurveFilter (@"aqua");
                break;
            case 7: 
                _filter = new GPUImageToneCurveFilter (@"yellow-red");
                break;
            case 8: 
                _filter = new GPUImageToneCurveFilter (@"06");
                break;
            case 9: 
                _filter = new GPUImageToneCurveFilter (@"purple-green");
                break;
            default:
                _filter = new GPUImageFilter ();
                break;
            }
            
            
        }

        void RemoveAllTargets ()
        {
            if (_stillCamera != null) {
            _stillCamera.RemoveAllTargets ();
        }
            if (_staticPicture != null) {
            _staticPicture.RemoveAllTargets ();
        }
            if (_cropFilter != null) {
            _cropFilter.RemoveAllTargets ();
        }

            
            //regular filter
            if (_filter != null) {
            _filter.RemoveAllTargets ();
        }

            
            //blur
            if (_blurFilter != null) {
            _blurFilter.RemoveAllTargets ();
        }


        }

        void SwitchToLibrary (object sender, EventArgs e)
        {
            if (!_isStatic) {
                _stillCamera.StopCameraCapture();
                RemoveAllTargets();
            }
            var imagePickerController = new UIImagePickerController {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                AllowsEditing = true
            };
            imagePickerController.Canceled += ImagePickerCancelled;
            imagePickerController.FinishedPickingMedia += ImagePickerDidFinish;
            PresentViewController (imagePickerController, true, null);
        }

        void RetakePhoto (object sender, EventArgs e)
        {
            _imagePickerView.RetakeButton.Hidden = true;
            _imagePickerView.LibraryButton.Hidden = false;
            _staticPicture = null;
            _staticPictureOriginalOrientation = UIImageOrientation.Up;
            _isStatic = false;
            RemoveAllTargets ();
            _stillCamera.StartCameraCapture ();
            _imagePickerView.ShutterButton.Enabled = true;
            if (UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera) && _stillCamera != null
                && _stillCamera.InputCamera.TorchAvailable) {
                _imagePickerView.FlashButton.Enabled = true;
            }
            _imagePickerView.ShutterButton.SetImage (UIImage.FromFile ("camera-icon.png"), UIControlState.Normal);
            _imagePickerView.ShutterButton.SetTitle (null, UIControlState.Normal);
            if (_imagePickerView.FiltersButton.Selected) {
                HideFilters();
            }
            SetFilter (_selectedFilter);
            PrepareFilter ();
        }




        void ImagePickerCancelled (object sender, EventArgs e)
        {
            var imagePickerController = (UIImagePickerController)sender;
            imagePickerController.Canceled -= ImagePickerCancelled;
            imagePickerController.FinishedPickingMedia -= ImagePickerDidFinish;
            if (_isStatic) {
                DismissViewController(false,()=> DismissViewController(false,()=>{}));
            } else {
                DismissViewController(false,()=>{});
                RetakePhoto(null,null);
            }
        }

        void ImagePickerDidFinish (object sender, UIImagePickerMediaPickedEventArgs e)
        {
            var imagePickerController = (UIImagePickerController)sender;
            imagePickerController.Canceled -= ImagePickerCancelled;
            imagePickerController.FinishedPickingMedia -= ImagePickerDidFinish;
            var outputImage = e.Info.ObjectForKey (UIImagePickerController.EditedImage) as UIImage ??
                              e.Info.ObjectForKey(UIImagePickerController.OriginalImage) as UIImage;
            if (outputImage != null) {
                _staticPicture = new GPUImagePicture(outputImage,true);
                _staticPictureOriginalOrientation = outputImage.Orientation;
                _isStatic = true;
                DismissViewController(true,null);
                _imagePickerView.FlipButton.Enabled = false;
                _imagePickerView.FlashButton.Enabled = false;
                PrepareStaticFilter();
                _imagePickerView.ShutterButton.SetTitle ("Done",UIControlState.Normal);
                _imagePickerView.ShutterButton.SetImage (null,UIControlState.Normal);
                _imagePickerView.ShutterButton.Enabled = true;
                if(!_imagePickerView.FiltersButton.Selected){
                    ShowFilters();
                }
            }
        }

        void FlipCameraView (object sender, EventArgs e)
        {
            _imagePickerView.FlipButton.Enabled = false;
            _stillCamera.RotateCamera ();
            _imagePickerView.FlipButton.Enabled = true;
            if (UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera)) {
                if(_stillCamera.InputCamera.FlashAvailable&&_stillCamera.InputCamera.TorchAvailable){
                    _imagePickerView.FlashButton.Enabled = true;
                }
                else{
                    _imagePickerView.FlashButton.Enabled = false;
                }
            }
        }

        void TakePhoto (object sender, EventArgs e)
        {
            _imagePickerView.ShutterButton.Enabled = false;
            if (!_isStatic) {
                _isStatic = true;
                _imagePickerView.FlashButton.Enabled = false;
                _imagePickerView.LibraryButton.Hidden = true;
                _imagePickerView.ShutterButton.Enabled = false;
                PrepareForCapture ();
            } else {
                GPUImageOutput processUpTo;
                if(_hasBlur){
                    processUpTo = _blurFilter;
                }
                else{
                    processUpTo = _filter;
                }
                _staticPicture.ProcessImage();
                UIImage currentFilteredVideoFrame = processUpTo.ImageFromCurrentlyProcessedOutputWithOrientation(_staticPictureOriginalOrientation);
                var data = new NSDictionary(currentFilteredVideoFrame.AsJPEG(),"data");
            }
        }

        void PrepareForCapture ()
        {
            NSError error;
            _stillCamera.InputCamera.LockForConfiguration(out error);
            if (_imagePickerView.FlashButton.Selected && _stillCamera.InputCamera.IsTorchModeSupported(AVCaptureTorchMode.On)) {
                _stillCamera.InputCamera.TorchMode = AVCaptureTorchMode.On;
                NSTimer.CreateScheduledTimer(0.25,CaptureImage);
            }
            else {
                CaptureImage();
            }
        }
        void CaptureImage()
        {
            var img = _cropFilter.ImageFromCurrentlyProcessedOutput ();
            _stillCamera.InputCamera.UnlockForConfiguration ();
            _stillCamera.StopCameraCapture ();
            RemoveAllTargets ();
            _staticPicture = new GPUImagePicture (img, true);
            _staticPictureOriginalOrientation = img.Orientation;
            PrepareFilter ();
            _imagePickerView.RetakeButton.Hidden = false;
            _imagePickerView.ShutterButton.SetTitle("Done",UIControlState.Normal);
            _imagePickerView.ShutterButton.SetImage (null, UIControlState.Normal);
            _imagePickerView.ShutterButton.Enabled = true;
            if (!_imagePickerView.FiltersButton.Selected) {
                ShowFilters();
            }

        }
        void ToggleFilters(object sender, EventArgs e){
            _imagePickerView.FiltersButton.Enabled = false;
            if (_imagePickerView.FiltersButton.Selected) {
                HideFilters();
            } else {
                ShowFilters();
            }

        }
        void ShowFilters ()
        {
            _imagePickerView.FiltersButton.Selected = true;
            _imagePickerView.FiltersButton.Enabled = false;
            var imageRect = _imagePickerView.ImageView.Frame;
            imageRect.Y -= 34;
            var sliderScrollFrame = _imagePickerView.FilterScrollView.Frame;
            sliderScrollFrame.Y -=  _imagePickerView.FilterScrollView.Frame.Height;
            var sliderScrollFrameBackground = _imagePickerView.FilterScrollBackgroundView.Frame;
            sliderScrollFrameBackground.Y -= _imagePickerView.FilterScrollBackgroundView.Frame.Height - 3;
            _imagePickerView.FilterScrollView.Hidden = false;
            _imagePickerView.FilterScrollBackgroundView.Hidden = false;
            UIView.Animate (0.10,
                           0.05,
                           UIViewAnimationOptions.CurveEaseOut,
                           () => {
                _imagePickerView.ImageView.Frame = imageRect;
                _imagePickerView.FilterScrollView.Frame = sliderScrollFrame;
                _imagePickerView.FilterScrollBackgroundView.Frame = sliderScrollFrameBackground;
            },
            () => {
                _imagePickerView.FiltersButton.Enabled = true;
            });
        }
        void HideFilters ()
        {
            _imagePickerView.FiltersButton.Selected = false;
            _imagePickerView.FiltersButton.Enabled = false;
            var imageRect = _imagePickerView.ImageView.Frame;
            imageRect.Y += 34;
            var sliderScrollFrame = _imagePickerView.FilterScrollView.Frame;
            sliderScrollFrame.Y += _imagePickerView.FilterScrollView.Frame.Height;
            var sliderScrollFrameBackground = _imagePickerView.FilterScrollBackgroundView.Frame;
            sliderScrollFrameBackground.Y += _imagePickerView.FilterScrollBackgroundView.Frame.Height - 3;

            UIView.Animate (0.10,
                            0.05,
                            UIViewAnimationOptions.CurveEaseOut,
                            () => {
                _imagePickerView.ImageView.Frame = imageRect;
                _imagePickerView.FilterScrollView.Frame = sliderScrollFrame;
                _imagePickerView.FilterScrollBackgroundView.Frame = sliderScrollFrameBackground;
            },() => {

                _imagePickerView.FiltersButton.Enabled = true;
                _imagePickerView.FilterScrollView.Hidden = true;
                _imagePickerView.FilterScrollBackgroundView.Hidden = true;
            });
        }

        void FocusCamera (object sender, UITapEventArgs e)
        {
            var tapGesture = e.Gesture;
            if (!_isStatic && tapGesture.State == UIGestureRecognizerState.Recognized) {
                var location = tapGesture.LocationInView(_imagePickerView.ImageView);
                var device = _stillCamera.InputCamera;
                var frameSize = _imagePickerView.ImageView.Frame.Size;
                if(_stillCamera.CameraPosition() == AVCaptureDevicePosition.Front)
                {
                    location.X = frameSize.Width-location.X;
                }
                var pointOfInterest = new PointF(location.Y/frameSize.Height,1f-(location.X/frameSize.Width));
                if(device.FocusPointOfInterestSupported&&device.IsFocusModeSupported(AVCaptureFocusMode.ModeAutoFocus)){
                    NSError error;
                    if(device.LockForConfiguration(out error)){
                        device.FocusPointOfInterest = pointOfInterest;
                        device.FocusMode = AVCaptureFocusMode.ModeAutoFocus;
                        if(device.ExposurePointOfInterestSupported&&device.IsExposureModeSupported(AVCaptureExposureMode.AutoExpose)){
                            device.ExposurePointOfInterest = pointOfInterest;
                            device.ExposureMode = AVCaptureExposureMode.AutoExpose;
                        }
                        _imagePickerView.FocusView.Center = tapGesture.LocationInView(View);
                        _imagePickerView.FocusView.Alpha = 1;
                        UIView.Animate(0.5,0.5,UIViewAnimationOptions.AllowAnimatedContent,()=>{
                            _imagePickerView.FocusView.Alpha = 0;
                        },null);
                        device.UnlockForConfiguration();
                    }
                    else{
                        Console.Write(error.ToString());
                    }
                }
            }
        }

        void ToggleFlash (object sender, EventArgs e)
        {
            _imagePickerView.FlashButton.Selected = !_imagePickerView.FlashButton.Selected;
        }

        void HandlePinch (object sender, UIPinchEventArgs e)
        {

        }
    }
}
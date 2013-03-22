using System;
using GPUImage;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public class UIPinchEventArgs : EventArgs
    {
        readonly UIPinchGestureRecognizer _gesture;

        public UIPinchEventArgs(UIPinchGestureRecognizer gesture)
        {
            _gesture = gesture;
        }
        public UIPinchGestureRecognizer Gesture
        {
            get
            {
                return _gesture;
            }
        }
    }
    public class UITapEventArgs : EventArgs
    {
        readonly UITapGestureRecognizer _gesture;

        public UITapEventArgs(UITapGestureRecognizer gesture)
        {
            _gesture = gesture;
        }
        public UITapGestureRecognizer Gesture
        {
            get
            {
                return _gesture;
            }
        }
    }
    public class ImagePickerView : UIView
    {
        GPUImageView _imageView;

        UIView _photoBar;

        UIView _topToolBar;

        UIButton _shutterButton;

        UIButton _filterButton;

        UIButton _libraryButton;

        UIButton _flashButton;

        UIButton _flipButton;

        UIButton _blurButton;

        UIButton _closeButton;

        UIButton _retakeButton;

        UIScrollView _filterScrollView;

        UIPinchGestureRecognizer _pinchGestureRecogniser;

        UITapGestureRecognizer _tapGestureRecogniser;

        UIImageView _focusView;

        UIImageView _filterScrollBackgroundView;

        BlurOverlayView _blurOverlayView;
        UIImageView _logoImageView;
        UIView _topBarView;

        public event EventHandler<UIPinchEventArgs> ImagePinched;
        public event EventHandler<UITapEventArgs> ImageTapped;
        public event EventHandler<EventArgs> LibraryButtonPressed;
        public event EventHandler<EventArgs> FlipButtonPressed;
        public event EventHandler<EventArgs> ShutterButtonPressed;
        public event EventHandler<EventArgs> CloseButtonPressed;
        public event EventHandler<EventArgs> RetakeButtonPressed;
        public event EventHandler<EventArgs> FilterButtonPressed;
        public event EventHandler<EventArgs> FlashButtonPressed;
        public ImagePickerView()
        {
            InitView();
            SetupEvents();
        }


        void InitView()
        {
            BackgroundColor = UIColor.White;
            _photoBar = new UIView {
                BackgroundColor = UIColor.FromRGBA(17,186,188,90)
            };
            _logoImageView = new UIImageView
            {
                Image = UIImage.FromFile(@"logo_small.png")
            };
            
            _closeButton = new UIButton();
            _closeButton.SetImage(UIImage.FromFile("close.png"), UIControlState.Normal);
            _topBarView = new UIView
            {
                BackgroundColor = UIColor.FromRGB(17, 186, 188)

            };
            _topBarView.Layer.ShadowOffset = new System.Drawing.SizeF(0, 2);
            _topBarView.Layer.ShadowRadius = 5;
            _topBarView.Layer.ShadowOpacity = 0.5f;
            //_topBarView.Layer.CornerRadius = 5;
        

            _topBarView.AddSubview(_logoImageView);
            _topBarView.AddSubview(_closeButton);
            _imageView = new GPUImageView(){BackgroundColor = UIColor.Black};
            _topToolBar = new UIView {BackgroundColor = UIColor.FromRGBA(239,237,236,0.9f)};
            SetupPhotoBar();
            SetupTopBar();
            _focusView = new UIImageView(UIImage.FromFile("focus-crosshair.png")) {Alpha = 0};
            _blurOverlayView = new BlurOverlayView(new System.Drawing.RectangleF(
                0, 0, _imageView.Frame.Width, _imageView.Frame.Height));
            _imageView.AddSubview(_blurOverlayView);



            _filterScrollView = new UIScrollView {
                ContentInset = new UIEdgeInsets {Bottom = 0, Top = 0, Left = 0, Right = 0},
                ScrollIndicatorInsets = new UIEdgeInsets {Bottom = 0, Top = 0, Left = 0, Right = 0},
                ScrollEnabled = true,
                BouncesZoom = true,
                DelaysContentTouches = true,
                CanCancelContentTouches = true,
                UserInteractionEnabled = true,
                ClipsToBounds = true,
                AutosizesSubviews = true,
                ClearsContextBeforeDrawing = true
            };


            _filterScrollBackgroundView = new UIImageView(UIImage.FromFile("dock_bg.png")) {
                Opaque = true,
                ClearsContextBeforeDrawing = true,
                AutosizesSubviews = true
            };


           
            AddSubview(_imageView);
           // AddSubview(_filterScrollBackgroundView);
           // AddSubview(_filterScrollView);

            AddSubview(_photoBar);
            AddSubview(_focusView);
            AddSubview(_topToolBar);
            AddSubview(_topBarView);

            _pinchGestureRecogniser = new UIPinchGestureRecognizer(OnPinch);
            _tapGestureRecogniser = new UITapGestureRecognizer(OnTap);
            _imageView.AddGestureRecognizer(_tapGestureRecogniser);


        }

        void SetupEvents()
        {
            _libraryButton.TouchUpInside += OnLibraryPress;
            _flipButton.TouchUpInside += OnFlipPress;
            _shutterButton.TouchUpInside += OnShutterPress;
            _closeButton.TouchUpInside += OnClosePress;
            _retakeButton.TouchUpInside += OnRetakePress;
            _filterButton.TouchUpInside += OnFilterButtonPress;
            _flashButton.TouchUpInside += OnFlashButtonPress;
        }

        void RemoveEvents()
        {
            _libraryButton.TouchUpInside -= OnLibraryPress;
            _flipButton.TouchUpInside -= OnFlipPress;
            _shutterButton.TouchUpInside -= OnShutterPress;
            _closeButton.TouchUpInside -= OnClosePress;
            _retakeButton.TouchUpInside -= OnRetakePress;
            _filterButton.TouchUpInside -= OnFilterButtonPress;
            _flashButton.TouchUpInside -= OnFlashButtonPress;
        }

        protected override void Dispose(bool disposing)
        {

            RemoveEvents();
            _pinchGestureRecogniser.Dispose();
            _tapGestureRecogniser.Dispose();
            _pinchGestureRecogniser = null;
            _tapGestureRecogniser = null;
            _topToolBar = null;
            _imageView = null;
            _libraryButton = null;
            _blurButton = null;
            _blurOverlayView = null;
            _closeButton = null;
            _filterButton = null;
            _flashButton = null;
            _flipButton = null;
            _focusView = null;
            _photoBar = null;
            _shutterButton = null;
            _topToolBar = null;
            base.Dispose(disposing);
        }


        void SetupTopBar()
        {
            _flashButton = new UIButton();
            _flashButton.SetImage(UIImage.FromFile("flash-off.png"), UIControlState.Normal);
            _flashButton.SetImage(UIImage.FromFile("flash.png"), UIControlState.Selected);
            _flashButton.Layer.BorderWidth=1;
            _flashButton.Layer.BorderColor = UIColor.FromRGBA(0, 0, 0, 80).CGColor;
            _flashButton.BackgroundColor = UIColor.Clear;
            
            _topToolBar.AddSubview(_flashButton);
            
            _blurButton = new UIButton();
            _blurButton.SetImage(UIImage.FromFile("blur.png"), UIControlState.Normal);
            _blurButton.SetImage(UIImage.FromFile("blur-on.png"), UIControlState.Selected);
            _blurButton.Layer.BorderWidth = 1;
            _blurButton.Layer.BorderColor = UIColor.FromRGBA(0, 0, 0, 80).CGColor;
            _blurButton.Selected = false;
            _topToolBar.AddSubview(_blurButton);
            
            _filterButton = new UIButton();
            _filterButton.SetImage(UIImage.FromFile("filter-open.png"), UIControlState.Normal);
            _filterButton.SetImage(UIImage.FromFile("filter-close.png"), UIControlState.Selected);
            _filterButton.Layer.BorderWidth = 1;
            _filterButton.Layer.BorderColor = UIColor.FromRGBA(0, 0, 0, 80).CGColor;
            _filterButton.Selected = false;
            _topToolBar.AddSubview(_filterButton);
        }

        void SetupPhotoBar()
        {
            _shutterButton = new UIButton();
            _shutterButton.SetImage(UIImage.FromFile("camera-button.png"), UIControlState.Normal);
           // _shutterButton.SetBackgroundImage(UIImage.FromFile("camera-button.png"), UIControlState.Normal);
          
            _libraryButton = new UIButton();
            _libraryButton.SetImage(UIImage.FromFile("library.png"), UIControlState.Normal);
            _retakeButton = new UIButton();
            _retakeButton.SetTitle("Retake", UIControlState.Normal);
            _retakeButton.Hidden = true;
            _retakeButton.SetBackgroundImage(UIImage.FromFile("camera-button.png"), UIControlState.Normal);
            
            _flipButton = new UIButton();
            _flipButton.SetImage(UIImage.FromFile("front-camera.png"), UIControlState.Normal);

            _photoBar.AddSubview(_flipButton);
            _photoBar.AddSubview(_shutterButton);
            _photoBar.AddSubview(_libraryButton);
           // _photoBar.AddSubview(_retakeButton);
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = Bounds.Width;
            var height = Bounds.Height;
            _topBarView.Frame = new System.Drawing.RectangleF(0, 0, width, 51);
            _logoImageView.Frame = new System.Drawing.RectangleF((width / 2) - (94 / 2), 3, 94, 44);
            _closeButton.Frame = new System.Drawing.RectangleF(width-15-(25/2), 15, 25, 22);
            _topToolBar.Frame = new System.Drawing.RectangleF(0, 51, width, (141/2));
            var toolBarButtonWidth = width/3;
            _flashButton.Frame = new System.Drawing.RectangleF(toolBarButtonWidth * 2, 0, toolBarButtonWidth, (141 / 2));
            _blurButton.Frame = new System.Drawing.RectangleF(toolBarButtonWidth, 0, toolBarButtonWidth, 141 / 2);
            _filterButton.Frame = new System.Drawing.RectangleF(0, 0, toolBarButtonWidth, 141/2);



            _closeButton.Frame = new System.Drawing.RectangleF(277, 3, 40, 37);
            _imageView.Frame = new System.Drawing.RectangleF(0, 51,width, height-51);
            _photoBar.Frame = new System.Drawing.RectangleF(0, height - 110, width, 110);

            var midpoint = width/2;
            _shutterButton.Frame = new System.Drawing.RectangleF(midpoint-(77/2),(110/2)-(78/2), 77, 78);

            

            _flipButton.Frame = new System.Drawing.RectangleF((midpoint+((midpoint/2)-(50/2)))+15, (110 / 2) - (50 / 2), 50, 50);

            _libraryButton.Frame = new System.Drawing.RectangleF(((midpoint / 2) - (50 / 2)) - 15, (110 / 2) - (50 / 2), 50, 50);

            _retakeButton.Frame = new System.Drawing.RectangleF(11, 7, 71, 29);
            _filterScrollView.Frame = new System.Drawing.RectangleF(0, 437, 320, 75);
            _filterScrollBackgroundView.Frame = new System.Drawing.RectangleF(-12, 435, 344, 75);

        }

        public UIButton ShutterButton
        {
            get
            {
                return _shutterButton;
            }
        }

        public UIButton FlipButton
        {
            get
            {
                return _flipButton;
            }
        }

        public UIButton FlashButton
        {
            get
            {
                return _flashButton;
            }
        }

        public UIButton LibraryButton
        {
            get
            {
                return _libraryButton;
            }
        }

        public UIButton RetakeButton
        {
            get
            {
                return _retakeButton;
            }
        }

        public UIButton FiltersButton
        {
            get
            {
                return _filterButton;
            }
        }
        public UIScrollView FilterScrollView
        {
            get
            {
                return _filterScrollView;
            }
        }
        public UIImageView FilterScrollBackgroundView
        {
            get
            {
                return _filterScrollBackgroundView;
            }
        }
        public UIImageView FocusView
        {
            get
            {
                return _focusView;
            }
        }
        public GPUImageView ImageView
        {
            get
            {
                return _imageView;
            }
        }


        void OnLibraryPress(object sender, EventArgs e)
        {
            if (LibraryButtonPressed != null)
            {
                LibraryButtonPressed.Invoke(this, e);
            }
        }

        void OnFlipPress(object sender, EventArgs e)
        {
            if (FlipButtonPressed != null)
            {
                FlipButtonPressed.Invoke(this, e);
            }
        }

        void OnShutterPress(object sender, EventArgs e)
        {
            if (ShutterButtonPressed != null)
            {
                ShutterButtonPressed.Invoke(this, e);
            }
        }

        void OnClosePress(object sender, EventArgs e)
        {
            if (CloseButtonPressed != null)
            {
                CloseButtonPressed.Invoke(this, e);
            }
        }

        void OnRetakePress(object sender, EventArgs e)
        {
            if (RetakeButtonPressed != null)
            {
                RetakeButtonPressed.Invoke(this, e);
            }
        }

        void OnFilterButtonPress(object sender, EventArgs e)
        {
            if (FilterButtonPressed != null)
            {
                FilterButtonPressed.Invoke(this, e);
            }
        }

        void OnFlashButtonPress(object sender, EventArgs e)
        {
            if (FlashButtonPressed != null)
            {
                FlashButtonPressed.Invoke(this, e);
            }
        }

        void OnPinch(UIPinchGestureRecognizer gesture)
        {
            if (ImagePinched != null)
            {
                ImagePinched.Invoke(this, new UIPinchEventArgs(gesture));
            }

        }

        void OnTap(UITapGestureRecognizer gesture)
        {
            if (ImageTapped != null)
            {
                ImageTapped.Invoke(this, new UITapEventArgs(gesture));
            }
        }
    }
}
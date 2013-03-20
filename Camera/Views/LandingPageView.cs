using System;
using System.Drawing;
using Camera.Views.Interfaces;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public sealed class LandingPageView : UIView, ILandingPageView
    {
        UIImageView _backgroundFrame;
        UIImageView _logoImageView;
        UIImageView _taglineView;

        UIImageView _taglineView2;

        UIButton _findButton;

        UIButton _createButton;

        FindNearbyMapView _mapView;

        UIButton _backButton;

        UIButton _findNearbyButton;

        UIButton _infoButton;

        UIButton _joinButton;

        UIButton _myEventsButton;

        UIView _landingView;

        UIImageView _eventIdView;

        UITextField _eventCodeField;

        int _extraOffset;

        int _viewOffset;
        int _currentBackground;


        public LandingPageView()
        {
            _currentBackground =1;
            BackgroundColor = UIColor.Black;
            Layer.CornerRadius = 5f;
            BecomeFirstResponder();
            InitViews();
            WireEvents();
            NSTimer.CreateRepeatingTimer(5, SwitchBackground);
        }

        public FindNearbyMapView MapView
        {
            get { return _mapView; }
        }

        void SwitchBackground()
        {
            if (_currentBackground >= 3)
            {
                _currentBackground = 1;
            }
            else
            {
                _currentBackground++;
            }
            _backgroundFrame.Image = UIImage.FromFile(String.Format(@"background{0}.png", _currentBackground));

        }

        public override void LayoutSubviews()
        {

            base.LayoutSubviews();
            //make the root view full size
            var mainBounds = Bounds;
            var height = Bounds.Size.Height;
            var width = Bounds.Size.Width;

            //	this.Frame = new System.Drawing.RectangleF(0,this.Bounds.Y-viewOffset,width,height);
            if (_viewOffset != 0)
            {
                mainBounds.Y = mainBounds.Y - _viewOffset;
            }
            if (_extraOffset != 0)
            {
                mainBounds.Y = mainBounds.Y - _extraOffset;
            }
            _landingView.Frame = mainBounds;
            _backgroundFrame.Frame = _landingView.Bounds;
            _mapView.Frame = Bounds;
            //landscape
            var horizontalMiddle = width / 2;
            _logoImageView.Frame = width > height ? 
                new RectangleF((horizontalMiddle - (136 / 2)) + 2, 20 + _viewOffset - _extraOffset, 136, 61) : 
                new RectangleF((horizontalMiddle - (271 / 2)) + 1, 81 + _viewOffset - _extraOffset, 271, 122);
            _taglineView.Frame = new RectangleF((horizontalMiddle - (236 / 2)), _logoImageView.Frame.Y + _logoImageView.Frame.Height + 10, 236, 18);
            _taglineView2.Frame = new RectangleF((horizontalMiddle - (204 / 2)), _logoImageView.Frame.Y + _logoImageView.Frame.Height + 10, 204, 32);
            _findButton.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _taglineView.Frame.Y + _taglineView.Frame.Height + 20, 234, 63);


            _createButton.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _findButton.Frame.Y + _findButton.Frame.Height + 5, 234, 63);
            _myEventsButton.Frame = new RectangleF((horizontalMiddle-(234/2)),_createButton.Frame.Y+_createButton.Frame.Height+5,234,63);

            _findNearbyButton.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _mapView.Frame.Height - 63 - 10, 234, 63);
            _backButton.Frame = new RectangleF(10, 10 + _viewOffset + _extraOffset, 25, 25);
            _infoButton.Frame = new RectangleF(width - 23 - 10, height - 23 - 10, 23, 23);
            _eventIdView.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _taglineView.Frame.Y + _taglineView.Frame.Height + 20, 234, 63);
            var fieldBounds = _eventIdView.Bounds;
            _eventCodeField.Frame = new RectangleF(0, fieldBounds.Y + 15, fieldBounds.Width, fieldBounds.Height - 20);
            _joinButton.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _eventIdView.Frame.Y + _eventIdView.Frame.Height + 5, 234, 63);
        }

        void InitViews()
        {
            _landingView = new UIView();
            _backgroundFrame = new UIImageView
            {
                Image = UIImage.FromFile(@"background1.png"),
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };
            _logoImageView = new UIImageView
            {
                Image = UIImage.FromFile(@"logo.png"),
                ContentMode = UIViewContentMode.ScaleAspectFit,
            };
            _taglineView = new UIImageView
            {
                Image = UIImage.FromFile(@"tagline.png"),
                ContentMode = UIViewContentMode.Center
            };

            _taglineView2 = new UIImageView
            {
                Image = UIImage.FromFile(@"tagline2.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = true
            };
            _findButton = new UIButton(new RectangleF(0,0,234,63));
            _createButton = new UIButton();
            _infoButton = new UIButton {Alpha =  0.7f};
            _myEventsButton = new UIButton();
            _joinButton = new UIButton { Hidden = true };
            _eventIdView = new UIImageView
            {
                Image = UIImage.FromFile(@"Field_Background.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = true,
                UserInteractionEnabled = true

            };

            _eventCodeField = new UITextField
            {
                Font = UIFont.FromName("ProximaNova-Bold", 24),
                Placeholder = "Event Code",
                TextAlignment = UITextAlignment.Center,
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ReturnKeyType = UIReturnKeyType.Join,
                AutocapitalizationType = UITextAutocapitalizationType.AllCharacters,

            };

            _eventIdView.AddSubview(_eventCodeField);
            _eventCodeField.ShouldReturn = delegate {
                return true;
            };

            _mapView = new FindNearbyMapView();
            _findNearbyButton = new UIButton();
            _backButton = new UIButton { Hidden = true };
            _findButton.SetBackgroundImage(UIImage.FromFile(@"Button_Find.png"), UIControlState.Normal);
            _createButton.SetBackgroundImage(UIImage.FromFile(@"Button_Create.png"), UIControlState.Normal);
            _myEventsButton.SetBackgroundImage(UIImage.FromFile(@"Button_MyEvents.png"),UIControlState.Normal);
            _infoButton.SetBackgroundImage(UIImage.FromFile(@"Button_Info.png"), UIControlState.Normal);
            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Back.png"), UIControlState.Normal);
            _findNearbyButton.SetBackgroundImage(UIImage.FromFile(@"Button_FindNearby.png"), UIControlState.Normal);
            _joinButton.SetBackgroundImage(UIImage.FromFile(@"Button_Join.png"), UIControlState.Normal);

            _landingView.ClipsToBounds = true;
            ClipsToBounds = true;
            AutosizesSubviews = true;
            //_backgroundFrame.AutoresizingMask = UIViewAutoresizing.FlexibleHeight|UIViewAutoresizing.FlexibleWidth;

            AddSubview(_mapView);
            _mapView.AddSubview(_findNearbyButton);

            AddSubview(_landingView);

            _landingView.AddSubview(_backgroundFrame);
            _landingView.AddSubview(_logoImageView);
            _landingView.AddSubview(_taglineView);
            _landingView.AddSubview(_findButton);
            _landingView.AddSubview(_createButton);
            _landingView.AddSubview(_myEventsButton);
            _landingView.AddSubview(_backButton);
            _landingView.AddSubview(_infoButton);
            _landingView.AddSubview(_taglineView2);
            _landingView.AddSubview(_eventIdView);
            _landingView.AddSubview(_joinButton);
        }
        public UITextField EventCodeField
        {
            get { return _eventCodeField; }
        }
        void OnFindButtonPress(object sender, EventArgs e)
        {

            if (FindButtonPressed != null)
            {
                
                FindButtonPressed.Invoke(this, e);
            }
        }
        void OnCreateButtonPress(object sender, EventArgs e)
        {
            if (CreateButtonPressed != null)
            {
                CreateButtonPressed.Invoke(this, e);
            }
        }

        void OnMyEventButtonPress(object sender, EventArgs e)
        {
            if (MyEventsButtonPressed != null)
            {
                MyEventsButtonPressed.Invoke(this, e);
            }
        }
        void OnBackButtonPress(object sender, EventArgs e)
        {
            if (BackButtonPressed != null)
            {
                BackButtonPressed.Invoke(this, e);
            }
        }

        void OnFindNearbyPress(object sender, EventArgs e)
        {
            if (FindNearbyButtonPressed != null)
            {
                FindNearbyButtonPressed.Invoke(this, e);
            }
        }

        void OnEventCodeEditBegin(object sender, EventArgs e)
        {
            _extraOffset = 20;
            LayoutSubviews();
        }

        void OnEventCodeEditEnd(object sender, EventArgs e)
        {
            _extraOffset = 0;
            _joinButton.Enabled = false;
            LayoutSubviews();
            OnJoinPress(sender, e);
            _eventCodeField.ResignFirstResponder();
        }

        void OnJoinPress(object sender, EventArgs e)
        {
            if (JoinButtonPressed != null)
            {
                JoinButtonPressed.Invoke(this, e);
            }
        }

        void WireEvents()
        {
            _findButton.TouchUpInside += OnFindButtonPress;
            _createButton.TouchUpInside += OnCreateButtonPress;
            _myEventsButton.TouchUpInside	 += OnMyEventButtonPress;
            _backButton.TouchUpInside += OnBackButtonPress;
            _findNearbyButton.TouchUpInside += OnFindNearbyPress;
            _eventCodeField.EditingDidBegin += OnEventCodeEditBegin;
            _eventCodeField.EditingDidEndOnExit += OnEventCodeEditEnd;
            _joinButton.TouchUpInside += OnJoinPress;

        }

        public void ShowFindNearby()
        {
            Animate(0.2, () =>
                {
                    _findButton.Alpha = 0;
                    _createButton.Alpha = 0;
                    _taglineView.Alpha = 0;
                    _infoButton.Alpha = 0;
                    _myEventsButton.Alpha = 0;
                }, () =>
                {
                    _findButton.Hidden = true;
                    _createButton.Hidden = true;
                    _taglineView.Hidden = true;
                    _infoButton.Hidden = true;
                    _myEventsButton.Hidden = true;
                    _viewOffset = 100;
                    Animate(0.5, LayoutSubviews, () =>
                        {
                            _backButton.Alpha = 0;
                            _taglineView2.Alpha = 0;
                            _eventIdView.Alpha = 0;
                            _joinButton.Alpha = 0;
                            _backButton.Hidden = false;
                            _findNearbyButton.Hidden = false;
                            _taglineView2.Hidden = false;
                            _eventIdView.Hidden = false;
                            _joinButton.Hidden = false;
                            Animate(0.2,() =>
                                {
                                    _backButton.Alpha = 0.7f;
                                    _taglineView2.Alpha = 1;
                                    _eventIdView.Alpha = 1;
                                    _joinButton.Alpha = 1;
                                });
                        });
                });
        }

        public void HideFindNearby()
        {
            _extraOffset = 0;
            _eventCodeField.Text = "";
            _eventCodeField.ResignFirstResponder();
            Animate(0.2, () =>
                {  
                    _backButton.Alpha = 0;
                    _taglineView2.Alpha = 0;
                    _eventIdView.Alpha = 0;
                    _joinButton.Alpha = 0;
                }, () =>
                    {
                        _backButton.Hidden = true;
                        _taglineView2.Hidden = true;
                        _eventIdView.Hidden = true;
                        _joinButton.Hidden = true;
                        _viewOffset = 0;
                        Animate(0.5, LayoutSubviews, () =>
                            {
                                _findButton.Alpha = 0;
                                _createButton.Alpha = 0;
                                _taglineView.Alpha = 0;
                                _infoButton.Alpha = 0;
                                _myEventsButton.Alpha = 0;
                                _findButton.Hidden = false;
                                _createButton.Hidden = false;
                                _taglineView.Hidden = false;
                                _infoButton.Hidden = false;
                                _myEventsButton.Hidden = false;
                                Animate(0.2, () =>
                                    {
                                        _findButton.Alpha = 1;
                                        _createButton.Alpha = 1;
                                        _taglineView.Alpha = 1;
                                        _infoButton.Alpha = 1;
                                        _myEventsButton.Alpha = 1;
                                    });
                            });
                    }
                );
        }

        public void AnimateToFullMap(Action onFinish)
        {
            _viewOffset = (int)Bounds.Height;
            _findNearbyButton.Hidden = true;
            Animate(0.5, LayoutSubviews, onFinish.Invoke);
        }
        public UIButton JoinButton
        {
            get { return _joinButton; }
        }



        void UnwireEvents()
        {

            _findButton.TouchUpInside -= OnFindButtonPress;
            _createButton.TouchUpInside -= OnCreateButtonPress;
            _backButton.TouchUpInside -= OnBackButtonPress;
            _findNearbyButton.TouchUpInside -= OnFindNearbyPress;
            _myEventsButton.TouchUpInside -= OnMyEventButtonPress;
            _eventCodeField.EditingDidBegin -= OnEventCodeEditBegin;
            _eventCodeField.EditingDidEndOnExit -= OnEventCodeEditEnd;
            _joinButton.TouchUpInside += OnJoinPress;
        }

        protected override void Dispose(bool disposing)
        {
            UnwireEvents();
            _backgroundFrame = null;
            _logoImageView = null;
            _taglineView = null;
            _findButton = null;
            _createButton = null;
            _backButton = null;
            _findNearbyButton = null;
            _landingView = null;
            _mapView = null;
            _myEventsButton = null;
        }
    
        public event EventHandler<EventArgs> FindButtonPressed;
        public event EventHandler<EventArgs> CreateButtonPressed;
        public event EventHandler<EventArgs> MyEventsButtonPressed;
        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> FindNearbyButtonPressed;
        public event EventHandler<EventArgs> JoinButtonPressed;
    }
}

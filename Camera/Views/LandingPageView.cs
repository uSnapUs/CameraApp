using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camera.Views.Interfaces;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public class LandingPageView : UIView, ILandingPageView
    {
        UIImageView _backgroundFrame;
        UIImageView _logoImageView;
        UIImageView _taglineView;

        UIImageView _taglineView2;

        UIButton _findButton;

        UIButton _createButton;


        UIButton _backButton;

        UIButton _findNearbyButton;

        UIButton _infoButton;

        UIButton _joinButton;


        UIView _landingView;

        UIImageView _eventIdView;

        UITextField _eventIdField;

        int _extraOffset;

        int viewOffset;


        public LandingPageView()
            : base()
        {
            BecomeFirstResponder();
            this.InitViews();
            this.WireEvents();
        }
     
        public override void LayoutSubviews()
        {

            base.LayoutSubviews();
            //make the root view full size
            var mainBounds = this.Bounds;
            var height = this.Bounds.Size.Height;
            var width = this.Bounds.Size.Width;

            //	this.Frame = new System.Drawing.RectangleF(0,this.Bounds.Y-viewOffset,width,height);
            if (viewOffset != 0)
            {
                mainBounds.Y = mainBounds.Y - viewOffset;
            }
            if (_extraOffset != 0)
            {
                mainBounds.Y = mainBounds.Y - _extraOffset;
            }
            this._landingView.Frame = mainBounds;
            this._backgroundFrame.Frame = this._landingView.Bounds;
            //landscape
            var horizontalMiddle = width / 2;
            if (width > height)
            {
                _logoImageView.Frame = new System.Drawing.RectangleF((horizontalMiddle - (136 / 2)) + 2, 20 + viewOffset - _extraOffset, 136, 61);

            }
            else
            {
                _logoImageView.Frame = new System.Drawing.RectangleF((horizontalMiddle - (271 / 2)) + 1, 81 + viewOffset - _extraOffset, 271, 122);

            }
            _taglineView.Frame = new System.Drawing.RectangleF((horizontalMiddle - (236 / 2)), _logoImageView.Frame.Y + _logoImageView.Frame.Height + 10, 236, 18);
            _taglineView2.Frame = new System.Drawing.RectangleF((horizontalMiddle - (204 / 2)), _logoImageView.Frame.Y + _logoImageView.Frame.Height + 10, 204, 32);
            _findButton.Frame = new System.Drawing.RectangleF((horizontalMiddle - (234 / 2)), _taglineView.Frame.Y + _taglineView.Frame.Height + 20, 234, 63);


            _createButton.Frame = new System.Drawing.RectangleF((horizontalMiddle - (234 / 2)), _findButton.Frame.Y + _findButton.Frame.Height + 5, 234, 63);
            //_myEventsButton.Frame = new System.Drawing.RectangleF((horizontalMiddle-(234/2)),_createButton.Frame.Y+_createButton.Frame.Height+5,234,63);

            _backButton.Frame = new System.Drawing.RectangleF(10, 10 + viewOffset + _extraOffset, 25, 25);
            _infoButton.Frame = new System.Drawing.RectangleF(width - 23 - 10, height - 23 - 10, 23, 23);
            _eventIdView.Frame = new System.Drawing.RectangleF((horizontalMiddle - (234 / 2)), _taglineView.Frame.Y + _taglineView.Frame.Height + 20, 234, 63);
            var fieldBounds = _eventIdView.Bounds;
            _eventIdField.Frame = new System.Drawing.RectangleF(0, fieldBounds.Y + 15, fieldBounds.Width, fieldBounds.Height - 20);
            _joinButton.Frame = new System.Drawing.RectangleF((horizontalMiddle - (234 / 2)), _eventIdView.Frame.Y + _eventIdView.Frame.Height + 5, 234, 63);
        }

        void InitViews()
        {
            _landingView = new UIView();
            _backgroundFrame = new UIImageView
            {
                Image = UIImage.FromFile(@"background.png"),
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
            _findButton = new UIButton();
            _createButton = new UIButton();
            _infoButton = new UIButton();
            _joinButton = new UIButton() { Hidden = true };
            _eventIdView = new UIImageView
            {
                Image = UIImage.FromFile(@"Field_Background.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = true,
                UserInteractionEnabled = true

            };

            _eventIdField = new UITextField
            {
                Font = UIFont.FromName("ProximaNova-Bold", 24),
                Placeholder = "Event Code",
                TextAlignment = UITextAlignment.Center,
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ReturnKeyType = UIReturnKeyType.Join,
                AutocapitalizationType = UITextAutocapitalizationType.AllCharacters,

            };

            _eventIdView.AddSubview(_eventIdField);
            _eventIdField.ShouldReturn = delegate(UITextField textField)
            {
                return true;
            };
            _findNearbyButton = new UIButton { Hidden = true };
            _backButton = new UIButton { Hidden = true };
            _findButton.SetBackgroundImage(UIImage.FromFile(@"Button_Find.png"), UIControlState.Normal);
            _createButton.SetBackgroundImage(UIImage.FromFile(@"Button_Create.png"), UIControlState.Normal);
            //_myEventsButton.SetBackgroundImage(UIImage.FromFile(@"Button_MyEvents.png"),UIControlState.Normal);
            _infoButton.SetBackgroundImage(UIImage.FromFile(@"Button_Info.png"), UIControlState.Normal);
            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Back.png"), UIControlState.Normal);
            _findNearbyButton.SetBackgroundImage(UIImage.FromFile(@"Button_FindNearby.png"), UIControlState.Normal);
            _joinButton.SetBackgroundImage(UIImage.FromFile(@"Button_Join.png"), UIControlState.Normal);

            this._landingView.ClipsToBounds = true;
            this.ClipsToBounds = true;
            this.AutosizesSubviews = true;
            //_backgroundFrame.AutoresizingMask = UIViewAutoresizing.FlexibleHeight|UIViewAutoresizing.FlexibleWidth;

            this.AddSubview(_landingView);

            _landingView.AddSubview(_backgroundFrame);
            _landingView.AddSubview(_logoImageView);
            _landingView.AddSubview(_taglineView);
            _landingView.AddSubview(_findButton);
            _landingView.AddSubview(_createButton);
            _landingView.AddSubview(_backButton);
            _landingView.AddSubview(_infoButton);
            _landingView.AddSubview(_taglineView2);
            _landingView.AddSubview(_eventIdView);
            _landingView.AddSubview(_joinButton);
        }

        void OnFindButtonPress(object sender, EventArgs e)
        {

            if (this.FindButtonPressed != null)
            {
                this.FindButtonPressed.Invoke(this, e);
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

        void OnEventIdEditBegin(object sender, EventArgs e)
        {
            _extraOffset = 20;
            LayoutSubviews();
        }

        void OnEventIdEditEnd(object sender, EventArgs e)
        {
            _extraOffset = 0;
            this._joinButton.Enabled = false;
            LayoutSubviews();
            this.OnJoinPress(sender, e);
            this._eventIdField.ResignFirstResponder();
        }

        void OnJoinPress(object sender, EventArgs e)
        {
            if (this.JoinButtonPressed != null)
            {
                JoinButtonPressed.Invoke(this, e);
            }
        }

        void WireEvents()
        {
            _findButton.TouchUpInside += OnFindButtonPress;
            _createButton.TouchUpInside += OnCreateButtonPress;
            //_myEventsButton.Unbind	 += OnMyEventButtonPress;
            _backButton.TouchUpInside += OnBackButtonPress;
            _findNearbyButton.TouchUpInside += OnFindNearbyPress;
            _eventIdField.EditingDidBegin += OnEventIdEditBegin;
            _eventIdField.EditingDidEndOnExit += OnEventIdEditEnd;
            _joinButton.TouchUpInside += OnJoinPress;

        }

        public void ShowFindNearby()
        {
            this._findButton.Hidden = true;
            this._createButton.Hidden = true;
            this._taglineView.Hidden = true;
            this._infoButton.Hidden = true;
            viewOffset = 100;
            UIView.Animate(0.5, this.LayoutSubviews, () =>
            {
                this._backButton.Hidden = false;
                this._findNearbyButton.Hidden = false;
                this._taglineView2.Hidden = false;
                this._eventIdView.Hidden = false;
                this._joinButton.Hidden = false;
            });
        }

        public void HideFindNearby()
        {

            _extraOffset = 0;
            this._eventIdField.Text = "";
            this._eventIdField.ResignFirstResponder();
            this._backButton.Hidden = true;
            this._findNearbyButton.Hidden = true;
            this._taglineView2.Hidden = true;
            this._eventIdView.Hidden = true;
            this._joinButton.Hidden = true;
            viewOffset = 0;
            UIView.Animate(0.5, this.LayoutSubviews, () =>
            {

                this._findButton.Hidden = false;
                this._createButton.Hidden = false;
                this._taglineView.Hidden = false;
                this._infoButton.Hidden = false;

            });
        }

        public void AnimateToFullMap(Action onFinish)
        {
            viewOffset = (int)this.Bounds.Height;
            _findNearbyButton.Hidden = true;
            Animate(0.5, this.LayoutSubviews, onFinish.Invoke);
        }





        void UnwireEvents()
        {

            _findButton.TouchUpInside -= OnFindButtonPress;
            _createButton.TouchUpInside -= OnCreateButtonPress;
            _backButton.TouchUpInside -= OnBackButtonPress;
            _findNearbyButton.TouchUpInside -= OnFindNearbyPress;
            _eventIdField.EditingDidBegin -= OnEventIdEditBegin;
            _eventIdField.EditingDidEndOnExit -= OnEventIdEditEnd;
            _joinButton.TouchUpInside += OnJoinPress;
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Disposing landing page view");
            this.UnwireEvents();
            _backgroundFrame = null;
            _logoImageView = null;
            _taglineView = null;
            _findButton = null;
            _createButton = null;
            _backButton = null;
            _findNearbyButton = null;
            _landingView = null;
        }

        public event EventHandler<EventArgs> FindButtonPressed;
        public event EventHandler<EventArgs> CreateButtonPressed;
        public event EventHandler<EventArgs> MyEventsButtonPressed;
        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<EventArgs> FindNearbyButtonPressed;
        public event EventHandler<EventArgs> JoinButtonPressed;
    }
}

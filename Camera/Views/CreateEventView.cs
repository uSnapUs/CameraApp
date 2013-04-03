using System;
using System.Drawing;
using Camera.Views.Interfaces;
using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace Camera.Views
{
    public class CreateEventView : UIView
    {
        public CreateEventView()
        {
            InitView();
            WireEvents();
        }

        UIImageView _backgroundFrame;

        UIView _landingView;

        UIImageView _logoImageView;

        UIButton _backButton;

        public event EventHandler<EventArgs> BackButtonPressed;

        public event EventHandler<EventArgs> LocationLookupViewShown;

        protected virtual void OnLocationLookupViewShown()
        {
            EventHandler<EventArgs> handler = LocationLookupViewShown;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        UIImageView _eventNameView;

        UITextField _eventNameField;

        UIImageView _locationView;

        UITextField _locationField;

        UIImageView _eventCodeView;

        UITextField _eventDateField;

        UIButton _publicToggle;

        UIButton _createButton;
        CalendarMonthView _calendarView;
        LocationLookupView _locationLookupView;

        void InitView()
        {
            var tapRecogniser = new UITapGestureRecognizer(OnTap) {
                ShouldReceiveTouch = ShouldReceiveTouch
            };
            AddGestureRecognizer(tapRecogniser);
            

            BecomeFirstResponder();
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

            _backButton = new UIButton();

            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Back.png"), UIControlState.Normal);



            //event name field
            _eventNameView = new UIImageView
            {
                Image = UIImage.FromFile(@"Field_Background.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = false,
                UserInteractionEnabled = true

            };

            _eventNameField = new UITextField {
                Font = UIFont.FromName("ProximaNova-Bold", 24),
                Placeholder = "Event Name",
                TextAlignment = UITextAlignment.Center,
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ReturnKeyType = UIReturnKeyType.Next,
                AutocapitalizationType = UITextAutocapitalizationType.Words,
                ShouldReturn = delegate {
                        return true;
                    },

            };


            _eventNameView.AddSubview(_eventNameField);

            //location field
            _locationView = new UIImageView
            {
                Image = UIImage.FromFile(@"Field_Background.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = false,
                UserInteractionEnabled = true

            };

            _locationField = new UITextField {
                Font = UIFont.FromName("ProximaNova-Bold", 24),
                Placeholder = "Location",
                TextAlignment = UITextAlignment.Center,
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ReturnKeyType = UIReturnKeyType.Next,
                AutocapitalizationType = UITextAutocapitalizationType.Words,
                ShouldReturn = delegate {
                        return true;
                    },
                AdjustsFontSizeToFitWidth = true
            };
            


            _locationView.AddSubview(_locationField);


            //event code field
            _eventCodeView = new UIImageView
            {
                Image = UIImage.FromFile(@"Field_Background.png"),
                ContentMode = UIViewContentMode.Center,
                Hidden = false,
                UserInteractionEnabled = true

            };
            CreateCalendarView();
            _eventDateField = new UITextField {
                Font = UIFont.FromName("ProximaNova-Bold", 24),
                Placeholder = "Event Date",
                TextAlignment = UITextAlignment.Center,
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ReturnKeyType = UIReturnKeyType.Next,
            };
            
            
            

            _eventCodeView.AddSubview(_eventDateField);

            _publicToggle = new UIButton
            {
                AdjustsImageWhenHighlighted = false
            };
            _publicToggle.SetBackgroundImage(UIImage.FromFile(@"button_toggle_public.png"), UIControlState.Normal);
            _publicToggle.SetBackgroundImage(UIImage.FromFile(@"button_toggle_private.png"), UIControlState.Selected);

            _createButton = new UIButton();
            _createButton.SetBackgroundImage(UIImage.FromFile(@"Button_Create_Blue.png"), UIControlState.Normal);
            _landingView.ClipsToBounds = true;
            ClipsToBounds = true;
            AutosizesSubviews = true;
            //_backgroundFrame.AutoresizingMask = UIViewAutoresizing.FlexibleHeight|UIViewAutoresizing.FlexibleWidth;




            AddSubview(_landingView);

            _landingView.AddSubview(_backgroundFrame);
            _landingView.AddSubview(_logoImageView);
            _landingView.AddSubview(_backButton);
            _landingView.AddSubview(_eventNameView);
            _landingView.AddSubview(_locationView);
            _landingView.AddSubview(_eventCodeView);
            _landingView.AddSubview(_publicToggle);
            _landingView.AddSubview(_createButton);
            AddSubview(_calendarView);
        }

        bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            var tapPoint = touch.LocationInView(_calendarView);
            if (tapPoint.Y > 0 && tapPoint.Y < _calendarView.Bounds.Height)
            {
                return false;
            }
            return true;
        }


        void CreateCalendarView()
        {
            if (_calendarView != null)
            {
                _calendarView.OnFinishedDateSelection=null;
            }
            _calendarView = new CalendarMonthView {
                OnFinishedDateSelection= OnFinishedDateSelection,
                IsDateAvailable = d => d > DateTime.Now
            };
        }

        void OnTap(UITapGestureRecognizer obj)
        {

            _eventDateField.ResignFirstResponder();
            _eventNameField.ResignFirstResponder();
            _locationField.ResignFirstResponder();
            HideCalendarView();
        }

        void OnFinishedDateSelection(DateTime dateTime)
        {
            _eventDateField.Text = dateTime.ToShortDateString();
            
            HideCalendarView();

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            //make the root view full size
            var mainBounds = Bounds;
            var height = Bounds.Size.Height;
            var width = Bounds.Size.Width;
             
            //	this.Frame = new System.Drawing.RectangleF(0,this.Bounds.Y-viewOffset,width,height);

            _landingView.Frame = mainBounds;
            _backgroundFrame.Frame = _landingView.Bounds;
            //landscape
            var horizontalMiddle = width / 2;
            _logoImageView.Frame = width > height ? new RectangleF((horizontalMiddle - (136 / 2)) + 2, 10, 136, 61) : new RectangleF((horizontalMiddle - (271 / 2)) + 1, 10, 271, 122);

            _backButton.Frame = new RectangleF(10, 10, 25, 25);
            _eventNameView.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _logoImageView.Frame.Y + _logoImageView.Frame.Height + 5, 234, 63);
            var fieldBounds = _eventNameView.Bounds;
            _eventNameField.Frame = new RectangleF(0, fieldBounds.Y + 15, fieldBounds.Width, fieldBounds.Height - 20);

            _locationView.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _eventNameView.Frame.Y + _eventNameView.Frame.Height + 3, 234, 63);
            fieldBounds = _locationView.Bounds;
            _locationField.Frame = new RectangleF(0, fieldBounds.Y + 15, fieldBounds.Width, fieldBounds.Height - 20);

            _eventCodeView.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _locationView.Frame.Y + _locationView.Frame.Height + 3, 234, 63);
            fieldBounds = _eventCodeView.Bounds;
            _eventDateField.Frame = new RectangleF(0, fieldBounds.Y + 15, fieldBounds.Width, fieldBounds.Height - 20);

            _publicToggle.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _eventCodeView.Frame.Y + _eventCodeView.Frame.Height + 3, 234, 58);
            _createButton.Frame = new RectangleF((horizontalMiddle - (234 / 2)), _publicToggle.Frame.Y + _publicToggle.Frame.Height + 20, 234, 63);
            
            _eventDateField.InputView = _calendarView;
            _calendarView.Center = new PointF(Center.X, Frame.Height + (_calendarView.Frame.Height / 2));
        }

        void UnwireEvents()
        {
            _backButton.TouchUpInside -= OnBackButtonPress;
            _publicToggle.TouchUpInside -= OnTogglePress;
            _eventDateField.EditingDidBegin -= EventDateFieldOnEditingDidBegin;
            _locationField.EditingDidBegin -= LocationFieldOnEditingDidBegin;
        }

       
        void HideCalendarView()
        {
            Animate(0.2, () =>
                {
                    _calendarView.Center = new PointF(Center.X, Frame.Height + (_calendarView.Frame.Height/2));
                });
        }

        void EventDateFieldOnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            _eventDateField.ResignFirstResponder();
            ShowCalendarView();
        }

        void ShowCalendarView()
        {
            Animate(0.2, () =>
                {
                    _calendarView.Center = Center;
                });
        }

        void WireEvents()
        {
            _backButton.TouchUpInside += OnBackButtonPress;
            _publicToggle.TouchUpInside += OnTogglePress;
            _eventDateField.EditingDidBegin += EventDateFieldOnEditingDidBegin;
            _locationField.EditingDidBegin += LocationFieldOnEditingDidBegin;
        }

        void LocationFieldOnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            _locationField.ResignFirstResponder();

            _locationLookupView = new LocationLookupView {
                Frame = new RectangleF(0, Bounds.Height, Bounds.Width, Bounds.Height)
            };

            AddSubview(_locationLookupView);
            Animate(0.2,() =>
                {
                    _locationLookupView.Center = new PointF(_locationLookupView.Center.X,Bounds.Height/2);
                },OnLocationLookupViewShown);
            
        }
        public MKMapView LocationLookupView
        {
            get { return _locationLookupView; }
        }
        protected override void Dispose(bool disposing)
        {
            UnwireEvents();
            _backButton = null;
            _backgroundFrame = null;
            _landingView = null;
            _logoImageView = null;
            _eventDateField = null;
            _eventCodeView = null;
            _eventNameField = null;
            _eventNameView = null;
            _locationField = null;
            _locationView = null;
            _publicToggle = null;
            _createButton = null;
            base.Dispose(disposing);
        }
        void OnBackButtonPress(object sender, EventArgs e)
        {
            _eventDateField.ResignFirstResponder();
            _eventNameField.ResignFirstResponder();
            _locationField.ResignFirstResponder();
            if (BackButtonPressed != null)
            {
                BackButtonPressed.Invoke(this, e);
            }
        }

        void OnTogglePress(object sender, EventArgs e)
        {
            _publicToggle.Selected = !_publicToggle.Selected;
        }
    }

    internal sealed class LocationLookupView : MKMapView,ILocationLookupView
    {
        

        public LocationLookupView()
        {

            ShowsUserLocation = true;

        }
        
    }
}
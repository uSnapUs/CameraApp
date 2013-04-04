using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Camera.Helpers;
using Camera.Views.Interfaces;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
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
        public event EventHandler<EventArgs> Search;

        public event EventHandler<EventArgs> Create;

        protected virtual void OnCreate()
        {
            EventHandler<EventArgs> handler = Create;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnSearch()
        {
            EventHandler<EventArgs> handler = Search;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnLocationLookupViewShown()
        {
            _eventNameField.ResignFirstResponder();
            _eventDateField.ResignFirstResponder();
            _locationField.ResignFirstResponder();
            HideCalendarView();

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
        AddressDetails _address;

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
            Date = dateTime;
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
            _publicToggle.TouchUpInside -= PublicToggleOnTouchUpInside;
            _createButton.TouchUpInside -= CreateButtonOnTouchUpInside;
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
            _publicToggle.TouchUpInside += PublicToggleOnTouchUpInside;
            _createButton.TouchUpInside += CreateButtonOnTouchUpInside;
        }

        void CreateButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            OnCreate();
        }

        void PublicToggleOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            _publicToggle.Selected = !_publicToggle.Selected;
        }

        void LocationFieldOnEditingDidBegin(object sender, EventArgs eventArgs)
        {
            
            _locationLookupView = new LocationLookupView {
                Frame = new RectangleF(0, Bounds.Height, Bounds.Width, Bounds.Height)
            };
            _locationLookupView.Search += LocationLookupViewOnSearch;
            _locationLookupView.Back += LocationLookupViewOnBack;
            _locationLookupView.AddressSelected += LocationLookupViewOnAddressSelected;
            AddSubview(_locationLookupView);
            Animate(0.2, () =>
                {
                    _locationLookupView.Center = new PointF(_locationLookupView.Center.X, Bounds.Height/2);
                }, OnLocationLookupViewShown);

    
            
        }

        void LocationLookupViewOnAddressSelected(object sender, AddressDetailsEventArgs addressDetailsEventArgs)
        {
            _address = addressDetailsEventArgs.Address;
            _locationField.Text = _address.Description;
            LocationLookupViewOnBack(null,null);
        }

        void LocationLookupViewOnBack(object sender, EventArgs eventArgs)
        {
            Animate(0.2,() =>
                {
                    _locationLookupView.Frame = new RectangleF(0, Bounds.Height, Bounds.Width, Bounds.Height);
                },() =>
                    {
                        _locationLookupView.RemoveFromSuperview();
                        _locationLookupView.Search -= LocationLookupViewOnSearch;
                        _locationLookupView.Back -= LocationLookupViewOnBack;
                        _locationLookupView.AddressSelected -= LocationLookupViewOnAddressSelected;
                        _locationLookupView.Dispose();
                        _locationLookupView = null;
                    });
        }

        void LocationLookupViewOnSearch(object sender, EventArgs eventArgs)
        {
            OnSearch();
        }

        public MKMapView LocationLookupView
        {
            get { return _locationLookupView; }
        }
        public string SearchText
        {
            get { return _locationLookupView.SearchText; }
        }

        public string Name
        {
            get { return _eventNameField.Text; }
            
        }

        public AddressDetails Address
        {
            get { return _address; }
            
        }

        public DateTime Date { get; set; }

        public bool Public
        {
            get { return _publicToggle.Selected; }
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

        public void AddAddressesToLocationMap(IEnumerable<AddressDetails> addresses)
        {
            if (_locationLookupView != null)
            {
                var addressDetails = addresses as AddressDetails[] ?? addresses.ToArray();
                if (addresses != null && addressDetails.Any())
                {
                    _locationLookupView.RemoveAnnotations(_locationLookupView.Annotations);
                    foreach (var annotation in addressDetails.Select(address =>new LocationMapAnnotation(address)))
                    {
                        _locationLookupView.AddAnnotation(annotation);
                    }

                    var firstLocation = addressDetails.First().Coordinate;
                    _locationLookupView.SetCenterCoordinate(new CLLocationCoordinate2D(firstLocation.Latitude,firstLocation.Longitude),true );
                }
            }
        }
    }

    public class LocationMapAnnotation:MKAnnotation
    {
        AddressDetails _address;

        public LocationMapAnnotation(AddressDetails address)
        {
            _address = address;
            Coordinate = new CLLocationCoordinate2D(address.Coordinate.Latitude,address.Coordinate.Longitude);
            
        }
        public override string Title
        {
            get { return _address.Description; }
        }
        public void SetTitle(string title)
        {
            _address.Description = title;
        }
        public override sealed CLLocationCoordinate2D Coordinate { get; set; }

        public AddressDetails AddressDetails
        {
            get { return _address; }
            set { _address = value; }
        }
    }

    internal sealed class LocationLookupView : MKMapView,ILocationLookupView
    {
        UITextField _searchBox;
        UIButton _searchButton;
        UIButton _backButton;
        readonly UITapGestureRecognizer _tapGestureRecogniser;
        readonly UITapGestureRecognizer _doubleTapGestureRecogniser;
        const string MapViewAnnotationIdentifier = "MapViewAnnotation";
        readonly UILongPressGestureRecognizer _longPressGestureRecogniser;
        public event EventHandler<EventArgs> Search;
        public event EventHandler<EventArgs> Back;
        public event EventHandler<AddressDetailsEventArgs> AddressSelected;

        void OnBack()
        {
            EventHandler<EventArgs> handler = Back;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public string SearchText
        {
            get { return _searchBox.Text; }
            
        }

        void OnSearch()
        {
            EventHandler<EventArgs> handler = Search;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public LocationLookupView()
        {
            _tapGestureRecogniser = new UITapGestureRecognizer(OnMapTap) {ShouldReceiveTouch = IsTapOnMap,CancelsTouchesInView = false};
            _doubleTapGestureRecogniser = new UITapGestureRecognizer(CreateAnnotation) {NumberOfTapsRequired = 2,CancelsTouchesInView = false};
            _longPressGestureRecogniser = new UILongPressGestureRecognizer(OnMapLongPress) {CancelsTouchesInView = false,
                MinimumPressDuration = 2,
            };
            CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
           AddGestureRecognizer(_tapGestureRecogniser);
            AddGestureRecognizer(_doubleTapGestureRecogniser);
            AddGestureRecognizer(_longPressGestureRecogniser);
            ShowsUserLocation = true;
            Delegate = new MyMapDelegate();
            InitSubviews();
            SetupEvents();
        }

        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs mkMapViewAccessoryTappedEventArgs)
        {
            Console.WriteLine(mkMapViewAccessoryTappedEventArgs.Control);
        }

        void OnMapLongPress()
        {
            Console.WriteLine("Long Press");
        }
        internal class MyMapDelegate : MKMapViewDelegate
        {
            MKReverseGeocoder _geoCoder;

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, NSObject annotation)
            {

                Console.WriteLine(annotation.GetType());
                var mapAnnotation = annotation as LocationMapAnnotation;
                if (mapAnnotation==null)
                    return null;
                var locationLookupView = mapView as LocationLookupView;
                var pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(MapViewAnnotationIdentifier);
                if (pinView == null)
                {
                    var accessoryButton = new UIButton(UIButtonType.DetailDisclosure);
                    if (locationLookupView != null)
                    {
                        accessoryButton.TouchUpInside += (s, e) => locationLookupView.SelectAddress(mapAnnotation.AddressDetails);
                    }
                    pinView = new MKPinAnnotationView(annotation, MapViewAnnotationIdentifier) {
                        PinColor = MKPinAnnotationColor.Green,
                        AnimatesDrop = true,
                        CanShowCallout = true,
                        LeftCalloutAccessoryView = accessoryButton,
                        
                        
                    };
                   
                    
                }
                else
                {
                    pinView.Annotation = annotation;

                }

                var coord = new CLLocationCoordinate2D(mapAnnotation.Coordinate.Latitude,
                                                       mapAnnotation.Coordinate.Longitude);
                if (_geoCoder != null)
                {
                    
                    _geoCoder.Cancel();
                    _geoCoder = null;
                    
                }
                _geoCoder = new MKReverseGeocoder(coord) {Delegate = new GeoCoderDelagate(pinView)};
                _geoCoder.Start();
                return pinView;
            }

           
        }

        void SelectAddress(AddressDetails addressDetails)
        {
            OnAddressSelected(addressDetails);
        }

        void OnAddressSelected(AddressDetails addressDetails)
        {
            var handler = AddressSelected;
            if (handler != null)
            {
                handler.Invoke(this,new AddressDetailsEventArgs {Address=addressDetails});
            }
        }

        bool IsTapOnMap(UIGestureRecognizer recognizer, UITouch touch)
        {
            var searchBoxLocation = touch.LocationInView(_searchBox);
            if (searchBoxLocation.X < 0 || searchBoxLocation.X > _searchBox.Frame.Width ||
                searchBoxLocation.Y < 0 || searchBoxLocation.Y > _searchBox.Frame.Height)
                _searchBox.ResignFirstResponder();

            return false;
        }

        void OnMapTap()
        {
           
        }

        void CreateAnnotation(UITapGestureRecognizer obj)
        {
            RemoveAnnotations(Annotations);
            var coord = ConvertPoint(obj.LocationInView(this), this);
            var annotation = new LocationMapAnnotation(
                new AddressDetails {
                    Coordinate = new Coordinate {Latitude = coord.Latitude, Longitude = coord.Longitude},
                    Description = "Custom Location"
                });
            
            AddAnnotation(annotation);
        }

        void SetupEvents()
        {
            _searchBox.EditingDidEnd += SearchBoxOnEditingDidEnd;
            _searchButton.TouchUpInside += SearchButtonOnTouchUpInside;
            _backButton.TouchUpInside += BackButtonOnTouchUpInside;
        }

        void BackButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            OnBack();
        }

        void SearchBoxOnEditingDidEnd(object sender, EventArgs eventArgs)
        {
            OnSearch();
        }

        void SearchButtonOnTouchUpInside(object sender, EventArgs eventArgs)
        {
            OnSearch();
        }

        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RemoveEvents();

        }

        void RemoveEvents()
        {
            
        }

        void InitSubviews()
        {
            _searchBox = new UITextField
            {
                BackgroundColor = UIColor.White,
                BorderStyle = UITextBorderStyle.RoundedRect,
                VerticalAlignment = UIControlContentVerticalAlignment.Center,
                ReturnKeyType = UIReturnKeyType.Search,
                EnablesReturnKeyAutomatically = true,
                ShouldReturn = field => true
            };
            
            _searchButton = new UIButton(UIButtonType.RoundedRect);
            _searchButton.SetTitle("Search",UIControlState.Normal);
            _backButton = new UIButton();

            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Back.png"), UIControlState.Normal);
            AddSubview(_searchBox);
            AddSubview(_searchButton);
            AddSubview(_backButton);
        }
       
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var width = Bounds.Width;
            _searchBox.Frame = new RectangleF(10,40,width-20-10-60,30);
            _searchButton.Frame = new RectangleF(width-10-60,45,60,20);
            _backButton.Frame = new RectangleF(10, 10, 25, 25);
        }
    }

    internal class AddressDetailsEventArgs : EventArgs
    {
        public AddressDetails Address { get; set; }
    }

    internal class GeoCoderDelagate : MKReverseGeocoderDelegate
    {
        readonly MKPinAnnotationView _annotationView;

        public GeoCoderDelagate(MKPinAnnotationView annotationView)
        {
            _annotationView = annotationView;
        }

        public override void FailedWithError(MKReverseGeocoder geocoder, NSError error)
        {
            Console.WriteLine("Failed to lookup location");
            Console.WriteLine(error);
        }

        public override void FoundWithPlacemark(MKReverseGeocoder geocoder, MKPlacemark placemark)
        {
            var mapAnnotation = _annotationView.Annotation as LocationMapAnnotation;
            if (mapAnnotation != null)
            {
                mapAnnotation.SetTitle(placemark.ToAddressDetails().Description);
            }
        }
    }
}
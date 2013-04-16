using System;
using System.Drawing;
using Camera.Model;
using Camera.Views.Interfaces;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using Camera.Helpers;
using Point = Camera.Model.Point;

namespace Camera.Views
{
    public class FindNearbyMapView : MKMapView, IFindNearbyMapView
    {
        UIView _topBarView;

        UIImageView _logoImageView;

        UIButton _backButton;
        public event EventHandler<EventArgs> BackButtonPressed;
        public event EventHandler<SelectedEventArgs> EventSelected;

        protected virtual void OnEventSelected(SelectedEventArgs e)
        {
            EventHandler<SelectedEventArgs> handler = EventSelected;
            if (handler != null) handler(this, e);
        }

        public FindNearbyMapView()
        {
            
            Delegate = new MyMapDelegate();
            
            InitViews();
            WireEvents();
        }

       

        public override void LayoutSubviews()
        {

            base.LayoutSubviews();
            //var height = this.Bounds.Height;
            var width = Bounds.Width;
            _topBarView.Frame = new System.Drawing.RectangleF(0, 0, width, 51);
            _logoImageView.Frame = new System.Drawing.RectangleF((width/2) - (94/2), 3, 94, 44);
            _backButton.Frame = new System.Drawing.RectangleF(15, 15, 25, 22);
        }

        void InitViews()
        {
           
            _logoImageView = new UIImageView {
                Image = UIImage.FromFile(@"logo_small.png")
            };
            _backButton = new UIButton();
            _backButton.SetBackgroundImage(UIImage.FromFile(@"Button_Topbar_Back.png"), UIControlState.Normal);
            _topBarView = new UIView
            {
                BackgroundColor = UIColor.FromRGB(17, 186, 188)

            };
            _topBarView.Layer.ShadowOffset = new System.Drawing.SizeF(0, 2);
            _topBarView.Layer.ShadowRadius = 5;
            _topBarView.Layer.ShadowOpacity = 0.5f;
            //_topBarView.Layer.CornerRadius = 5;
            AddSubview(_topBarView);

            _topBarView.AddSubview(_logoImageView);
            _topBarView.AddSubview(_backButton);
        }

        void OnBackButtonPress(object sender, EventArgs e)
        {
            if (BackButtonPressed != null)
            {
                BackButtonPressed.Invoke(this, e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            UnwireEvents();
            _topBarView = null;
            _logoImageView = null;
            _backButton = null;

        }

        void WireEvents()
        {

            _backButton.TouchUpInside += OnBackButtonPress;
        }

        void UnwireEvents()
        {

            _backButton.TouchUpInside -= OnBackButtonPress;
            
            
        }
        internal class MyMapDelegate : MKMapViewDelegate
        {
            public override void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
            {
                if (!_updatedLocation)
                {
                    mapView.SetCenterCoordinate(userLocation.Coordinate, 12, true);
                    mapView.ScrollEnabled = false;
                    _updatedLocation = true;
                }

            }
        
            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, NSObject annotation)
            {

                Console.WriteLine(annotation.GetType());
                var mapAnnotation = annotation as EventAnnotation;
                if (mapAnnotation == null)
                    return null;
                var locationLookupView = mapView as FindNearbyMapView;
                var pinView = mapView.DequeueReusableAnnotation(MapViewAnnotationIdentifier) as EventPinAnnotationView;
                if (pinView == null)
                {
                    var accessoryButton = new UIButton(UIButtonType.DetailDisclosure);
                    if (locationLookupView != null)
                    {
                        accessoryButton.TouchUpInside += (s, e) => locationLookupView.GoToEvent(mapAnnotation);
                    }
                    pinView = new EventPinAnnotationView(mapAnnotation, MapViewAnnotationIdentifier)
                    {


                    };


                }
                else
                {
                    pinView.SetEventAnnotation(mapAnnotation);

                }
                
               
                return pinView;
            }
            
            protected string MapViewAnnotationIdentifier = "EventAnnotation";
            bool _updatedLocation;
        }

        void GoToEvent(EventAnnotation ev)
        {
            Console.Write("Got to touch event");
            OnEventSelected(new SelectedEventArgs {Event = ev.Event});
        }
    }

    public sealed class EventPinAnnotationView : MKAnnotationView
    {
        EventAnnotation _annotation;
        UILabel _label;

        public EventPinAnnotationView(EventAnnotation annotation, string reuseIdentifier)
            : base(annotation, reuseIdentifier)
        {
            _annotation = annotation;
            CanShowCallout = false;
            var labelText = new NSString(_annotation.Event.Name);
            var labelSize = labelText.StringSize(UIFont.FromName("Proxima Nova", 34));
            _label = new UILabel(new RectangleF(new PointF(0-(labelSize.Width/2),0-labelSize.Height),labelSize )) { Text = _annotation.Event.Name, Font = UIFont.FromName("Proxima Nova", 34) };
            AddSubview(_label);
        }
        
        

        public void SetEventAnnotation(EventAnnotation mapAnnotation)
        {
            _annotation = mapAnnotation;
            Annotation = mapAnnotation;
        }
    }

    

    public class SelectedEventArgs : EventArgs
    {
        public Event Event { get; set; }
    }

    public class EventAnnotation : MKAnnotation
    {
        readonly Event _ev;

        public EventAnnotation(Event ev)
        {
            _ev = ev;
        }
        public override string Title
        {
            get { return _ev.Name; }
        }
        public override CLLocationCoordinate2D Coordinate
        {
            get
            {
                return
                    new CLLocationCoordinate2D(_ev.Location.Latitude, _ev.Location.Longitude);
            }
            set { _ev.Location = new Point { Longitude = value.Longitude, Latitude = value.Latitude }; }
        }

        public Event Event
        {
            get { return _ev; }
        }
    }
}
using System;
using System.Linq;
using Camera.Helpers;
using Camera.Model;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    public sealed class FindNearbyViewController:BaseViewController,IFindNearbyViewController
    {
        FindNearbyMapView _mapView;
        FindNearbyViewControllerSupervisor _supervisor;

        public FindNearbyViewController(FindNearbyMapView mapView)
        {
            _supervisor = new FindNearbyViewControllerSupervisor(this);
            _mapView = mapView;
            _mapView.ShowsUserLocation = true;
            _mapView.BackButtonPressed += OnBackButtonPressed;
            _mapView.EventSelected += MapViewOnEventSelected;
            View = new FindNearbyView(_mapView);
        }

        void MapViewOnEventSelected(object sender, SelectedEventArgs selectedEventArgs)
        {
            var eventDashboardViewController = new EventDashboardViewController(selectedEventArgs.Event);
            PresentViewController(eventDashboardViewController, true, Dispose);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UIApplication.SharedApplication.SetStatusBarHidden(false, true);
        }
        
        protected override void EnsureSupervised()
        {
            if (_supervisor == null)
            {
                _supervisor = new FindNearbyViewControllerSupervisor(this);
            }
        }
        
        public event EventHandler<EventArgs> BackButtonPressed;

        void OnBackButtonPressed(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = BackButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void GoToLandingView()
        {
            var landingView = new LandingPageViewController();
            PresentViewController(landingView, true, Dispose);
        }

        public Coordinate GetMapLocation()
        {
            var coord = _mapView.CenterCoordinate;
            return
                new Coordinate {
                    Longitude = coord.Longitude,
                    Latitude = coord.Latitude
                };

        }

        public void ShowEventAnnotations(Event[] eventsNearby)
        {
            _mapView.AddAnnotations(eventsNearby.Select(ev=> new EventAnnotation(ev)).ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            UnwireEvents();
            _supervisor = null;
            _mapView = null;
            base.Dispose(disposing);
            
        }

        void UnwireEvents()
        {
            _mapView.BackButtonPressed -= OnBackButtonPressed;

            _mapView.EventSelected-= MapViewOnEventSelected;
        }
    }

   
}
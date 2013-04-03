using System;
using Camera.Helpers;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.CoreLocation;

namespace Camera.ViewControllers
{
    public sealed class CreateEventViewController:BaseViewController,ICreateEventViewController
    {
        CreateEventView _createEventView;
        CreateEventViewControllerSupervisor _supervisor;

        public CreateEventViewController()
        {
            _createEventView = new CreateEventView();
            EnsureSupervised();
            View = _createEventView;
        }

        protected override void EnsureSupervised()
        {
            if (_supervisor == null)

            {
                _supervisor = new CreateEventViewControllerSupervisor(this);
                WireEvents();
            }

        }

        void WireEvents()
        {
            _createEventView.BackButtonPressed += OnBackPressed;
            _createEventView.LocationLookupViewShown += OnLocationLookupShown;
        }

        public void PresentLandingView()
        {
            var landingViewController = new LandingPageViewController();
            PresentViewController(landingViewController,true,Dispose);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _createEventView.BackButtonPressed -= OnBackPressed;
            _createEventView.LocationLookupViewShown -= OnLocationLookupShown;
            _createEventView = null;
            _supervisor = null;
        }
        public event EventHandler<EventArgs> BackPressed;
        public event EventHandler<EventArgs> LocationLookupShown;
        public void SetLocation(Coordinate coordinate)
        {
            var coord = new CLLocationCoordinate2D(coordinate.Latitude, coordinate.Longitude);
            _createEventView.LocationLookupView.SetCenterCoordinate(coord, 14, true);		
        }

        void OnLocationLookupShown(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = LocationLookupShown;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        void OnBackPressed(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = BackPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
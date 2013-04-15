using System;
using System.Collections.Generic;
using Camera.Helpers;
using Camera.Model;
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
        public override bool DisablesAutomaticKeyboardDismissal
        {
            get { return false; }
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
            _createEventView.Search += OnLocationSearch;
            _createEventView.Create += OnCreate;
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
            _createEventView.Search -= OnLocationSearch;
            _createEventView.Create -= OnCreate;
            _createEventView = null;
            _supervisor = null;
        }
        public event EventHandler<EventArgs> BackPressed;
        public event EventHandler<EventArgs> LocationLookupShown;
        public event EventHandler<EventArgs> LocationSearch;
        public string LocationSearchText { get { return _createEventView.SearchText; } }
        public string Name { get { return _createEventView.Name; }
        }

        public AddressDetails Location
        {
            get { return _createEventView.Address; }
        }

        public DateTime Date { get { return _createEventView.Date; } }
        public bool Public { get { return _createEventView.Public; } }
        public event EventHandler<EventArgs> Create;

        void OnCreate(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = Create;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void AddAddressesToLocationMap(IEnumerable<AddressDetails> addresses)
        {
            _createEventView.AddAddressesToLocationMap(addresses);
        }

        public void GoToEventDashboard(Event serverEvent)
        {
            var eventDashboardController = new EventDashboardViewController(serverEvent);
            PresentViewController(eventDashboardController,true,Dispose);
        }

        void OnLocationSearch(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = LocationSearch;
            if (handler != null) handler(this, EventArgs.Empty);
        }

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
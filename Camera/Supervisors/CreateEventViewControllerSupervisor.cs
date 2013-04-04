using System;
using System.Collections.Generic;
using Camera.Helpers;
using Camera.Model;
using Camera.ViewControllers.Interfaces;
using MonoTouch.UIKit;

namespace Camera.Supervisors
{
    public class CreateEventViewControllerSupervisor:BaseViewControllerSupervisor
    {
        readonly ICreateEventViewController _createEventViewController;
        Coordinate? _coordinate;

        public CreateEventViewControllerSupervisor(ICreateEventViewController viewController) : base(viewController)
        {
            
            _createEventViewController = viewController;
            _createEventViewController.BackPressed += CreateEventViewControllerOnBackPressed;
            _createEventViewController.LocationLookupShown += CreateEventViewControllerOnLocationLookupShown;
            _createEventViewController.LocationSearch += CreateEventViewControllerOnLocationSearch;
            _createEventViewController.Create += CreateEventViewControllerOnCreate;
        }

        void CreateEventViewControllerOnCreate(object sender, EventArgs eventArgs)
        {
            var name =_createEventViewController.Name;
            var location = _createEventViewController.Location;
            var date = _createEventViewController.Date;
            if (string.IsNullOrEmpty(name)||location.Equals(default(AddressDetails))||date==default(DateTime))
                return;
            var createdEvent = StateManager.Current.CreateEvent(new Event {
                Address = location.Description,
                Location = location.Coordinate,
                StartDate =date.Date,
                EndDate = date.Date.AddDays(1),
                IsPublic = _createEventViewController.Public,
                Name = name
            });
            _createEventViewController.GoToEventDashboard(createdEvent);
        }

        void CreateEventViewControllerOnLocationSearch(object sender, EventArgs eventArgs)
        {
            var searchText = _createEventViewController.LocationSearchText;
            if (string.IsNullOrEmpty(searchText))
                return;
            StateManager.Current.LocationManager.LookupAddress(_createEventViewController.LocationSearchText,OnAddressLookup);
        }

        void CreateEventViewControllerOnLocationLookupShown(object sender, EventArgs eventArgs)
        {
            StateManager.Current.LocationManager.SubscribeToLocationUpdates(OnLocationUpdated);
        }

        public void OnLocationUpdated(object sender, LocationEventArgs e)
        {
            if (_coordinate == null || _coordinate.Value.Accuracy < e.Coordinate.Accuracy)
                _coordinate = e.Coordinate;
            else
            {
                StateManager.Current.LocationManager.UnsubscribeFromLocationUpdates(OnLocationUpdated);
                _createEventViewController.SetLocation(_coordinate.Value);
            }
        }   

        void CreateEventViewControllerOnBackPressed(object sender, EventArgs eventArgs)
        {
            _createEventViewController.PresentLandingView();
        }
        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            _createEventViewController.BackPressed -= CreateEventViewControllerOnBackPressed;
            _createEventViewController.LocationLookupShown -= CreateEventViewControllerOnLocationLookupShown;
            _createEventViewController.LocationSearch -= CreateEventViewControllerOnLocationSearch;
            _createEventViewController.Create -= CreateEventViewControllerOnCreate;
        }

        public void OnAddressLookup(IEnumerable<AddressDetails> addresses)
        {
            _createEventViewController.AddAddressesToLocationMap(addresses);
        }
    }
}
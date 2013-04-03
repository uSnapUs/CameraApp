using System;
using Camera.Helpers;
using Camera.ViewControllers.Interfaces;

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
        }

        void CreateEventViewControllerOnLocationLookupShown(object sender, EventArgs eventArgs)
        {
            StateManager.Current.LocationManager.SubscribeToLocationUpdates(OnLocationUpdated);
        }

        void OnLocationUpdated(object sender, LocationEventArgs e)
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
        }
    }
}
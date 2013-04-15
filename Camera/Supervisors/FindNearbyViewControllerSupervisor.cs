using System;
using Camera.Helpers;
using Camera.ViewControllers.Interfaces;

namespace Camera.Supervisors
{
    public class FindNearbyViewControllerSupervisor:BaseViewControllerSupervisor
    {
        IFindNearbyViewController _findNearbyViewController;

        public FindNearbyViewControllerSupervisor(IFindNearbyViewController viewController) : base(viewController)
        {
            _findNearbyViewController = viewController;
            _findNearbyViewController.BackButtonPressed += FindNearbyViewControllerOnBackButtonPressed;
            _findNearbyViewController.Appear += FindNearbyViewControllerOnAppear;
        }

        void FindNearbyViewControllerOnAppear(object sender, EventArgs eventArgs)
        {
            var location = _findNearbyViewController.GetMapLocation();
            var eventsNearby = StateManager.Current.Server.FindEventsByLocation(location);
            _findNearbyViewController.ShowEventAnnotations(eventsNearby);
        }

        void FindNearbyViewControllerOnBackButtonPressed(object sender, EventArgs eventArgs)
        {
            _findNearbyViewController.GoToLandingView();
        }


        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            _findNearbyViewController.BackButtonPressed -= FindNearbyViewControllerOnBackButtonPressed;
            _findNearbyViewController.Appear -= FindNearbyViewControllerOnAppear;
        }

        public override void Dispose()
        {
            base.Dispose();
            _findNearbyViewController = null;
        }
    }
}
using System;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
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
            View = new FindNearbyView(_mapView);
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
        }
    }
}
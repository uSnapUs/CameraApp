using System;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Camera.ViewControllers
{
  

    [Register("LandingPageViewController")]
    public class LandingPageViewController : BaseViewController,ILandingPageViewController
    {
        LandingPageView _landingPageView;
        LandingPageViewControllerSupervisor _supervisor;
        LoginViewController _loginViewController;

        public LandingPageViewController()
        {
            
            _supervisor = new LandingPageViewControllerSupervisor(this);


        }
        protected override void EnsureSupervised()
        {
            if (_supervisor == null)
            {
                _supervisor = new LandingPageViewControllerSupervisor(this);
            }
        }
        public override void ViewDidLoad()
        {
            UIApplication.SharedApplication.SetStatusBarHidden(true, true);
            _landingPageView = new LandingPageView();
            View = _landingPageView;
            WireEvents();
            base.ViewDidLoad();
        }

        void WireEvents()
        {
            _landingPageView.MyEventsButtonPressed += LandingPageView_MyEventsButtonPressed;
            _landingPageView.CreateButtonPressed += LandingPageView_CreateButtonPressed;
            _landingPageView.FindButtonPressed += LandingPageView_FindButtonPressed;
            _landingPageView.BackButtonPressed += LandingPageView_BackButtonPressed;
            _landingPageView.FindNearbyButtonPressed += LandingPageView_FindNearbyButtonPressed;
            _landingPageView.JoinButtonPressed += LandingPageViewOnJoinButtonPressed;
        }

        void LandingPageViewOnJoinButtonPressed(object sender, EventArgs eventArgs)
        {
            OnJoinButtonPressed();
        }

        void LandingPageView_FindNearbyButtonPressed(object sender, EventArgs e)
        {
           OnFindNearbyButtonPressed();
        }

        void LandingPageView_BackButtonPressed(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        void LandingPageView_FindButtonPressed(object sender, EventArgs e)
        {
            OnFindButtonPressed();
        }

        void LandingPageView_CreateButtonPressed(object sender, EventArgs e)
        {
            OnCreateButtonPressed();
        }

        void LandingPageView_MyEventsButtonPressed(object sender, EventArgs e)
        {
            OnMyEventsButtonPressed();
        }

        public override void ViewDidDisappear(bool animated)
        {
            _supervisor = null;
            base.ViewDidDisappear(animated);
        }
        protected override void Dispose(bool disposing)
        {
            OnUnload();
            _supervisor = null;
            _landingPageView = null;
            base.Dispose(disposing);
        }

        

        

        public event EventHandler<EventArgs> FindButtonPressed;

        protected virtual void OnFindButtonPressed()
        {
            EventHandler<EventArgs> handler = FindButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs> CreateButtonPressed;

        protected virtual void OnCreateButtonPressed()
        {
            EventHandler<EventArgs> handler = CreateButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> MyEventsButtonPressed;

        protected virtual void OnMyEventsButtonPressed()
        {
            EventHandler<EventArgs> handler = MyEventsButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs> BackButtonPressed;

        protected virtual void OnBackButtonPressed()
        {
            EventHandler<EventArgs> handler = BackButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> FindNearbyButtonPressed;
        

        protected virtual void OnFindNearbyButtonPressed()
        {
            EventHandler<EventArgs> handler = FindNearbyButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs> JoinButtonPressed;

        protected virtual void OnJoinButtonPressed()
        {
            EventHandler<EventArgs> handler = JoinButtonPressed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public string EventCode
        {
            get { return _landingPageView.EventCodeField.Text; } 
            set { _landingPageView.EventCodeField.Text = value; }
        }


        public void PresentMyEventsView()
        {
            
        }

        public void PresentFindEventsView()
        {
            _landingPageView.ShowFindNearby();
        }

        public void PresentCreateView()
        {
            
        }

        public void HideFindEventsView()
        {
            _landingPageView.HideFindNearby();
        }

        public void PresentFindNearbyView()
        {
            _landingPageView.AnimateToFullMap(GoToMapView);
        }

        public void PresentLoginView(string loginReason)
        {
            _loginViewController = new LoginViewController(View);
        }

        public void ShowValidationMessage(string validationMessage)
        {
            _landingPageView.JoinButton.Enabled = true;
        }

        public void PresentEventDashboard()
        {
            var eventDashboardViewController = new EventDashboardViewController();
            PresentViewController(eventDashboardViewController,true,Dispose);
        }

        void GoToMapView()
        {
            var mapView = _landingPageView.MapView;
            var mapViewController = new FindNearbyViewController(mapView);
            PresentViewController(mapViewController,false,Dispose);
        }
    }
}
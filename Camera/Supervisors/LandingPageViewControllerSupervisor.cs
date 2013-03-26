using System;
using Camera.Helpers;
using Camera.ViewControllers.Interfaces;

namespace Camera.Supervisors
{
    public class LandingPageViewControllerSupervisor:BaseViewControllerSupervisor
    {
        ILandingPageViewController _viewController;

        public LandingPageViewControllerSupervisor(ILandingPageViewController viewController):base(viewController)
        {
            _viewController = viewController;

            _viewController.FindButtonPressed += ViewController_FindButtonPressed;
            _viewController.CreateButtonPressed += ViewController_CreateButtonPressed;
            _viewController.MyEventsButtonPressed += ViewController_MyEventsButtonPressed;
            _viewController.BackButtonPressed += ViewController_BackButtonPressed;
            _viewController.FindNearbyButtonPressed += ViewController_FindNearbyButtonPressed;
            _viewController.JoinButtonPressed += ViewControllerOnJoinButtonPressed;
        }

        void ViewControllerOnJoinButtonPressed(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_viewController.EventCode))
            {
               _viewController.ShowValidationMessage("Event code cannot be empty");
            }
            else
            {
                _viewController.PresentEventDashboard();
            }
        }

        void ViewController_FindNearbyButtonPressed(object sender, EventArgs e)
        {
            _viewController.PresentFindNearbyView();
        }

        void ViewController_BackButtonPressed(object sender, EventArgs e)
        {
            _viewController.HideFindEventsView();
        }

        void ViewController_MyEventsButtonPressed(object sender, EventArgs e)
        {
            if (!StateManager.Current.IsAuthenticated)
            {
                _viewController.PresentLoginView("looking up your events");
            }
            else
            {
                _viewController.PresentMyEventsView();
            }
        }

        void ViewController_CreateButtonPressed(object sender, EventArgs e)
        {
            if (!StateManager.Current.IsAuthenticated)
            {
                _viewController.PresentLoginView("creating an event");
            }
            else
            {
                _viewController.PresentCreateView();
            }
        }

        void ViewController_FindButtonPressed(object sender, EventArgs e)
        {
           _viewController.PresentFindEventsView();
        }

        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            _viewController.MyEventsButtonPressed -= ViewController_MyEventsButtonPressed;
            _viewController.CreateButtonPressed -= ViewController_CreateButtonPressed;
            _viewController.FindButtonPressed -= ViewController_FindButtonPressed;
            _viewController.BackButtonPressed -= ViewController_BackButtonPressed;
            _viewController.FindNearbyButtonPressed -= ViewController_FindNearbyButtonPressed;
            _viewController.JoinButtonPressed -= ViewControllerOnJoinButtonPressed;
        }

        public override void Dispose()
        {
            base.Dispose();
            _viewController = null;
        }

      
    }
}
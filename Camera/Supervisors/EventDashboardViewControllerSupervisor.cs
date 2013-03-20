using System;
using Camera.ViewControllers.Interfaces;

namespace Camera.Supervisors
{
    public class EventDashboardViewControllerSupervisor:BaseViewControllerSupervisor
    {
        IEventDashboardViewController _eventDashboardViewController;

        public EventDashboardViewControllerSupervisor(IEventDashboardViewController viewController) : base(viewController)
        {
            _eventDashboardViewController = viewController;
            _eventDashboardViewController.BackButtonPressed += EventDashboardViewControllerOnBackButtonPressed;
            _eventDashboardViewController.CameraButtonPressed += EventDashboardViewControllerOnCameraButtonPressed;
        }

        void EventDashboardViewControllerOnCameraButtonPressed(object sender, EventArgs eventArgs)
        {
            _eventDashboardViewController.PresentImagePickerView();
        }

        void EventDashboardViewControllerOnBackButtonPressed(object sender, EventArgs eventArgs)
        {
            _eventDashboardViewController.PresentLandingView();
        }
        public override void Dispose()
        {
            base.Dispose();
            _eventDashboardViewController.BackButtonPressed -= EventDashboardViewControllerOnBackButtonPressed;
            _eventDashboardViewController.CameraButtonPressed -= EventDashboardViewControllerOnCameraButtonPressed;
            _eventDashboardViewController = null;
        }
    }
}
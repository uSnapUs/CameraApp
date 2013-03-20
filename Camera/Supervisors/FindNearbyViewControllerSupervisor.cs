using System;
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
        }

        void FindNearbyViewControllerOnBackButtonPressed(object sender, EventArgs eventArgs)
        {
            _findNearbyViewController.GoToLandingView();
        }


        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            _findNearbyViewController.BackButtonPressed -= FindNearbyViewControllerOnBackButtonPressed;
        }

        public override void Dispose()
        {
            base.Dispose();
            _findNearbyViewController = null;
        }
    }
}
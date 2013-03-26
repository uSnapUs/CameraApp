using System;
using Camera.Helpers;
using Camera.ViewControllers.Interfaces;

namespace Camera.Supervisors
{
    public class LoginViewControllerSupervisor:BaseViewControllerSupervisor
    {
        ILoginViewController _loginViewController;

        public LoginViewControllerSupervisor(ILoginViewController loginViewController):base(loginViewController)
        {
            _loginViewController = loginViewController;
            _loginViewController.FacebookLoginPress += LoginViewControllerFacebookLoginPress;

            StateManager.Current.Authenticated += StateManagerOnAuthenticated;
        }

        void StateManagerOnAuthenticated(object sender, EventArgs eventArgs)
        {
            _loginViewController.Dismiss();
        }

        void LoginViewControllerFacebookLoginPress(object sender, System.EventArgs e)
        {
           StateManager.Current.InitiateFacebookLogin();
        }
        protected override void UnwireEvents()
        {
            base.UnwireEvents();
            _loginViewController.FacebookLoginPress -= LoginViewControllerFacebookLoginPress;
            StateManager.Current.Authenticated -= StateManagerOnAuthenticated;
            
        }
        public override void Dispose()
        {
            base.Dispose();
            _loginViewController = null;
        }
    }
}
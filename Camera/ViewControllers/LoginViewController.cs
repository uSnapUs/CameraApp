using System;
using Camera.Supervisors;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.ViewControllers
{
    [Register("LoginViewController")]
    public sealed class LoginViewController : BaseViewController,ILoginViewController
    {
        LoginViewControllerSupervisor _supervisor;
        LoginView _loginView;

        public LoginViewController(UIView rootView)
        {
            _loginView = new LoginView();

            View = _loginView;
            _supervisor = new LoginViewControllerSupervisor(this);
            rootView.AddSubview(View);
            _loginView.TransitionIn();
        }

      
    
        protected override void EnsureSupervised()
        {
            if (_supervisor == null)
            {
                _supervisor = new LoginViewControllerSupervisor(this);
            }
        }

        public override void ViewDidLoad()
        {
            _loginView = new LoginView();
            View = _loginView;
            EnsureSupervised();
            base.ViewDidLoad();

            // Perform any additional setup after loading the View
        }

        public event EventHandler<EventArgs> FacebookLoginPress;

        protected override void Dispose(bool disposing)
        {
            

            _loginView = null;
            _supervisor = null;
            base.Dispose(disposing);
            
        }
    }
}
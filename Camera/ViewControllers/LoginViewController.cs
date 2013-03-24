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

        public LoginViewController(UIView rootView,string viewText)
        {

            _loginView = new LoginView(viewText);

            View = _loginView;
            _loginView.FacebookLoginPressed += OnFacebookLoginPress;
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
            EnsureSupervised();
            base.ViewDidLoad();

            // Perform any additional setup after loading the View
        }

        public event EventHandler<EventArgs> FacebookLoginPress;

        void OnFacebookLoginPress(object sender, EventArgs eventArgs)
        {
            EventHandler<EventArgs> handler = FacebookLoginPress;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {

            _loginView.FacebookLoginPressed -= OnFacebookLoginPress;
            _loginView = null;
            _supervisor = null;
            base.Dispose(disposing);
            
        }
    }
}
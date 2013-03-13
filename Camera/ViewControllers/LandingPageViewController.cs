using System;
using System.Drawing;
using Camera.ViewControllers.Interfaces;
using Camera.Views;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Camera.ViewControllers
{
  

    [Register("LandingPageViewController")]
    public class LandingPageViewController : UIViewController,ILandingPageViewController
    {
        LandingPageView _landingPageView;

        public LandingPageViewController()
        {
            UIApplication.SharedApplication.SetStatusBarHidden(true, true);
            _landingPageView = new LandingPageView();
            this.View = _landingPageView;

        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
           
            base.ViewDidLoad();
            OnLoad();
            // Perform any additional setup after loading the view
        }

        public event EventHandler<EventArgs> Load;

        protected virtual void OnLoad()
        {
            EventHandler<EventArgs> handler = Load;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Unload;
        public event EventHandler<EventArgs> Appear;
        public event EventHandler<EventArgs> BeforeAppear;
        public event EventHandler<EventArgs> FindButtonPressed;
    }
}
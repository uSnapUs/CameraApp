using System;
using System.Collections.Generic;
using System.Linq;
using Camera.Helpers;
using Camera.ViewControllers;
using MonoTouch.CoreFoundation;
using MonoTouch.FacebookConnect;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public override UIWindow Window
        {
            get;
            set;
        }

        

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            app.SetStatusBarHidden(true, false);
            this.Window = new UIWindow(UIScreen.MainScreen.Bounds);
            var defaultViewController = new LandingPageViewController();
            this.Window.RootViewController = defaultViewController;
            this.Window.MakeKeyAndVisible();
            this.Window.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    StateManager.Current.DeviceName =
                        UIDevice.CurrentDevice.Name;
                   
                });
            return true;
        }


        public void SaveAuthorization()
        {
            
        }

        public void ClearAuthorization()
        {
            
        }
    }
}
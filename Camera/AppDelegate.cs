using System;
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
    public class AppDelegate : UIApplicationDelegate
    {
        
        public static string SCSessionStateChangedNotification = new NSString("us.usnap.camera.Login:FBSessionStateChangedNotification");

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
            MonoTouch.TestFlight.TestFlight.TakeOff("36e40e5d-ae3d-47da-bb3a-9d445c52c367");
            // create a new window instance based on the screen size
            app.SetStatusBarHidden(true, false);
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            var defaultViewController = new LandingPageViewController();
            Window.RootViewController = defaultViewController;
            Window.MakeKeyAndVisible();
            Window.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    StateManager.Current.DeviceName =
                        UIDevice.CurrentDevice.Name;
                   
                });

            return true;
        }


        
        public bool OpenSession (bool allowLoginUI)
        {
            var permissions = new[] { "email" };
            
            return 	FBSession.OpenActiveSession(permissions, allowLoginUI, OnSessionStateChanged);
        }
        public void OnSessionStateChanged(FBSession session, FBSessionState state, NSError error)
        {
            // FBSample logic
            // Any time the session is closed, we want to display the login controller (the user
            // cannot use the application unless they are logged in to Facebook). When the session
            // is opened successfully, hide the login controller and show the main UI.
            switch (state) 
            {
                
            case FBSessionState.Open:
                Console.WriteLine("open");
                FBRequestConnection.StartForMeWithCompletionHandler(LoggedInAs);
                break;
                
            case FBSessionState.Closed:
                Console.WriteLine("closed");
                FBSession.ActiveSession.CloseAndClearTokenInformation();
                break;
                
            case FBSessionState.ClosedLoginFailed:
                Console.WriteLine("Closed login failed");
                break;
            }
            
            NSNotificationCenter.DefaultCenter.PostNotificationName(SCSessionStateChangedNotification, session);
            
            if (error != null) 
            {
                var alertView = new UIAlertView("Error: " + ((FBErrorCode)error.Code).ToString(), error.LocalizedDescription, null, "Ok", null);
                alertView.Show();
            }
        }
        public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // FBSample logic
            // We need to handle URLs by passing them to FBSession in order for SSO authentication
            // to work.
            return FBSession.ActiveSession.HandleOpenURL(url);
        }
        
        public override void OnActivated (UIApplication application)
        {
            // FBSample logic
            // We need to properly handle activation of the application with regards to SSO
            //  (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
            try
            {
                FBSession.ActiveSession.HandleDidBecomeActive();
            }
            catch (Exception e)
            {
                MonoTouch.TestFlight.TestFlight.Log("Error on activated {0}",e.Message);
            }
        }

        void LoggedInAs (FBRequestConnection connection, NSObject result, NSError error)
        {



            if (error == null)
            {
                var user = new FBGraphUser(result);
                Console.WriteLine(user);
                Console.WriteLine(user.ObjectForKey((new NSString("email"))));
                StateManager.Current.UpdateDeviceRegistration(user.Name, user.ObjectForKey(new NSString("email")).ToString(),user.Id);
                
            }
        }
    }
}
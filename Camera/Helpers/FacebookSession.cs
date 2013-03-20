using System;
using MonoTouch.FacebookConnect;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.Helpers
{
    public class FacebookSession:IFacebookSession
    {
        static IFacebookSession _facebookSession;
        readonly Facebook _facebook;
        readonly SessionDelegate _sessionDelegate;
        readonly string _appId;

        public FacebookSession()
        {
            
            _appId = NSBundle.MainBundle.ObjectForInfoDictionary("FacebookAppID").ToString();
            _sessionDelegate = new SessionDelegate();
            _facebook = new Facebook(){Delegate = _sessionDelegate};
            var defaults = NSUserDefaults.StandardUserDefaults;
            if (defaults["FBAccessTokenKey"] != null && defaults["FBExpirationDateKey"] != null)
            {
                _facebook.AccessToken = defaults["FBAccessTokenKey"] as NSString;
                _facebook.ExpirationDate = defaults["FBExpirationDateKey"] as NSDate;
            }
            _sessionDelegate.Login += SessionDelegateLogin;
            ValidateSetup();
        }

        void SessionDelegateLogin(object sender, EventArgs e)
        {
            SaveAuthorization();
            FBRequest.GetRequestForMe.Start((connection, result, error) =>
                {
                    if (error == null)
                    {
                        FBGraphUser user = new FBGraphUser(result);
                        Console.WriteLine(user);
                        Console.WriteLine(user.Id);
                    }
                });
        }

        void SaveAuthorization()
        {
            var defaults = NSUserDefaults.StandardUserDefaults;
            if (_facebook.AccessToken != null)
            {
                defaults["FBAccessTokenKey"] = new NSString(_facebook.AccessToken);
            }
            if (_facebook.ExpirationDate != null)
            {
                defaults["FBExpirationDateKey"] = _facebook.ExpirationDate;
            }
            defaults.Synchronize();
        }


        // This method merely validates that the basic components of a Facebook
        // app are complete, it should not be needed in a complete application,
        // but will help in the first stages of debugging your Facebook
        // interop
        //
        void ValidateSetup()
        {
            if (_appId == null)
                SetupError("Missing AppId,  You can not run the app until this is setup");

            // Validate the callback "fb[APPID]://authorize is in the Info.plist
            // which is what facebook uses to call us back
            bool urlFound = false;
            try
            {
                var arr = NSArray.FromArray<NSObject>(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleURLTypes") as NSArray);
                if (arr != null && arr.Length > 0)
                {
                    var dict = arr[0] as NSDictionary;
                    arr = NSArray.FromArray<NSString>(dict[(NSString)"CFBundleURLSchemes"] as NSArray);
                    if (arr != null && arr.Length > 0)
                    {
                        var fbvalue = arr[0].ToString();
                        if (fbvalue.StartsWith("fb" + _appId))
                            urlFound = true;
                    }
                }
                else
                    SetupError("Missing fbXXXX URL handler in Info.plist's CFBundleURLTypes section");
            }
            catch
            {
                SetupError("Invalid contents of Info.plist file");
            }

            if (!urlFound)
                SetupError("Missing correct fbXXXX i the Info.plist's setup");

            // Check if the authorization callback will work
            if (!UIApplication.SharedApplication.CanOpenUrl(new NSUrl("fb" + _appId + "://authorize")))
                SetupError("Invalid or missing URL scheme. You cannot run the app until you set up a valid URL scheme in your .plist.");
        }
        void ShowMessage(string caption, string message, NSAction callback = null)
        {
            var alert = new UIAlertView(caption, message, null, "Ok");
            if (callback != null)
                alert.Dismissed += delegate { callback(); };
            alert.Show();
        }

        void SetupError(string msg)
        {
            ShowMessage("Setup Error", msg, () => Environment.Exit(1));
        }
        public static IFacebookSession Current
        {
            get { return _facebookSession ?? (_facebookSession = new FacebookSession()); }
            set { _facebookSession = value; }
        }

        public void InitiateLogin()
        {
            if (_facebook.IsSessionValid)
            {
                
            }
            else
            {
                _facebook.Authorize(new[]{"email"});
            }
        }

        public bool HandleOpenURL(NSUrl url)
        {
            Console.WriteLine("Got: {0} and {1}", url, _facebook.Handle);
            return _facebook.HandleOpenURL(url);
        }


        // 
        // The session FBSessionDelegate instance will let us get events informing us
        // when the user has logged in/logged out or when his session becomes invalid
        //
        class SessionDelegate : FBSessionDelegate
        {
            public event EventHandler<EventArgs> Login;
            event EventHandler<EventArgs> Logout;
            event EventHandler<EventArgs> NotLogin;
            event EventHandler<EventArgs> SessionWasInvalidated;
            public override void DidLogin()
            {
                EventHandler<EventArgs> handler = Login;
                if (handler != null) handler(this, EventArgs.Empty);
            }
            public override void DidLogout()
            {
                EventHandler<EventArgs> handler = Logout;
                if (handler != null) handler(this, EventArgs.Empty);
            }

            public override void DidNotLogin(bool cancelled)
            {
                EventHandler<EventArgs> handler = NotLogin;
                if (handler != null) handler(this, EventArgs.Empty);
            }

            public override void SessionInvalidated()
            {
                EventHandler<EventArgs> handler = SessionWasInvalidated;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }
    }
}
using System;
using MonoTouch.FacebookConnect;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.Helpers
{
    public class FacebookSession:IFacebookSession
    {
        Facebook _facebook;
        static IFacebookSession _currentSession;


        // 
        // The session FBSessionDelegate instance will let us get events informing us
        // when the user has logged in/logged out or when his session becomes invalid
        //
        public class SessionDelegate : FBSessionDelegate
        {
            AppDelegate container;
            NSAction onLogin;

            public NSAction OnLogin
            {
                get
                {
                    return this.onLogin;
                }
                set
                {
                    onLogin = value;
                }
            }

            public SessionDelegate(AppDelegate container)
            {
                this.container = container;
            }

            public override void DidNotLogin(bool cancelled)
            {
                Console.WriteLine("did not login");
                //container.SaveAuthorization ();
                if (OnLogin != null) OnLogin.Invoke();
            }

            public override void DidLogin()
            {
                Console.WriteLine("login !");
                container.SaveAuthorization();
                if (OnLogin != null) OnLogin.Invoke();
            }

            public override void DidLogout()
            {
                Console.WriteLine("logout !");
                container.ClearAuthorization();
            }
        }

        public FacebookSession()
        {
            var sessionDelegate = new SessionDelegate((AppDelegate)UIApplication.SharedApplication.Delegate);
            var appId = NSBundle.MainBundle.ObjectForInfoDictionary("FacebookAppID").ToString();
            _facebook = new Facebook(appId, sessionDelegate);
            var defaults = NSUserDefaults.StandardUserDefaults;
            if (defaults["FBAccessTokenKey"] != null && defaults["FBExpirationDateKey"] != null)
            {
                _facebook.AccessToken = defaults["FBAccessTokenKey"] as NSString;
                _facebook.ExpirationDate = defaults["FBExpirationDateKey"] as NSDate;
            }
        }

        public static IFacebookSession Current
        {
            get { return _currentSession ?? (_currentSession = new FacebookSession()); }
            set { _currentSession = value; }
        }


        public void InitiateLogin()
        {
            _facebook.Authorize(new[] { "email", "publish_stream", "read_friendlists", "user_photos" });    
        }
    }
}
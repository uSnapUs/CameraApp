using System;
using MonoTouch.FacebookConnect;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Camera.Helpers
{
    public class FacebookSession:IFacebookSession
    {
		static IFacebookSession _facebookSession;
		public static IFacebookSession Current
		{
			get{
				return _facebookSession??(_facebookSession = new FacebookSession());
			}
			set{
				_facebookSession = value;
			}
		}
		#region IFacebookSession implementation
		public void InitiateLogin ()
		{
			((AppDelegate)UIApplication.SharedApplication.Delegate).OpenSession(true);
		}
		#endregion
       
    }
}
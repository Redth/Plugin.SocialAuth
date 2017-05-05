using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace SocialAuthSample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			global::Plugin.SocialAuth.iOS.SocialAuth.Init ();
			global::Plugin.SocialAuth.Facebook.iOS.FacebookAuthProvider.Init ("YOUR-FB-APPID");
			global::Plugin.SocialAuth.Google.iOS.GoogleAuthProvider.Init ();

			LoadApplication(new App ());

			return base.FinishedLaunching(app, options);
		}
	}
}

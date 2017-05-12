using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.SocialAuth;
using Plugin.SocialAuth.Facebook.Native.iOS;
using Plugin.SocialAuth.Google.Native.iOS;
using Plugin.SocialAuth.iOS;
using Plugin.SocialAuth.Twitter.iOS;
using UIKit;

namespace SocialAuthSample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			// Init the things
			SocialAuth.Init();
			FacebookAuthProvider.Init(SocialKeys.FACEBOOK_APP_ID);

			//Register custom handlers
			SocialAuthManager.Current.RegisterProvider<FacebookAuthProvider>(ProviderType.Facebook);
			SocialAuthManager.Current.RegisterProvider<GoogleAuthProvider>(ProviderType.Google);
			SocialAuthManager.Current.RegisterProvider<TwitterSafariServicesAuthProvider>(ProviderType.Twitter);
			SocialAuthManager.Current.RegisterProvider<OAuth2SafariServicesProvider>("fitbit");

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		// We need to relay open url calls to social auth
		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (Plugin.SocialAuth.iOS.SocialAuth.OpenUrl(application, url, sourceApplication, annotation))
				return true;

			return false;
		}
	}
}

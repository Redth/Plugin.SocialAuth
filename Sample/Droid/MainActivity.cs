using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.SocialAuth.Droid;
using Plugin.SocialAuth.Facebook.Droid;
using Plugin.SocialAuth;
using Plugin.SocialAuth.Google.Droid;
using Plugin.SocialAuth.Twitter.Droid;
using Plugin.SocialAuth.Droid.CustomTabs;

namespace SocialAuthSample.Droid
{
	// We need this callback to redirect callbacks into our app to SocialAuth to handle
	[Activity(NoHistory = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
	[IntentFilter(new[] { Intent.ActionView },
				  Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "plugin.socialauth")]
	public class SocialAuthOAuth1CallbackActivity : Plugin.SocialAuth.Droid.CustomTabs.OAuthCallbackActivity
	{
	}

	[Activity(Label = "SocialAuthSample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			// Init the things
			SocialAuth.Init(this.Application);
			FacebookAuthProvider.Init(SocialKeys.FACEBOOK_APP_ID);

			//Register custom handlers
			SocialAuthManager.Current.RegisterProvider<FacebookAuthProvider>(ProviderType.Facebook);
			SocialAuthManager.Current.RegisterProvider<GoogleAuthProvider>(ProviderType.Google);
			SocialAuthManager.Current.RegisterProvider<TwitterCustomTabsAuthProvider>(ProviderType.Twitter);
			SocialAuthManager.Current.RegisterProvider<OAuth2CustomTabsProvider>("fitbit");

			LoadApplication(new App());
		}

		// Pass OnActivityResult back into social auth
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			Plugin.SocialAuth.Droid.SocialAuth.OnActivityResult(requestCode, resultCode, data);
		}
	}
}

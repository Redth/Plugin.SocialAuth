using System;
using Android.App;

namespace Plugin.SocialAuth.Droid.CustomTabs
{
	public class OAuthCallbackActivity : Activity
	{
		protected override void OnCreate(Android.OS.Bundle bundle)
		{
			base.OnCreate(bundle);

			SocialAuth.Callback(this.Intent, bundle);

			Finish();
		}
	}
}

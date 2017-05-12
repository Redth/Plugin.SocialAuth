using System;
using System.Threading.Tasks;
using Plugin.SocialAuth.Droid.CustomTabs;

namespace Plugin.SocialAuth.Twitter.Droid
{
	public class TwitterCustomTabsAuthProvider : OAuth1CustomTabsProvider
	{
		public override Task<OAuth.IOAuth1Account> AuthenticateAsync(OAuth.IOAuth1Options options)
		{
			// Retain original delegate to call also
			var originalProcessExtras = options.ProcessExtras;

			options.ProcessExtras = new Plugin.SocialAuth.OAuth.OAuth1ProcessExtrasDelegate((extras, acct) =>
			{
				// Get account extra information
				string userId = null;
				if (extras.TryGetValue("user_id", out userId))
					acct.Id = userId;

				string screenName = null;
				if (extras.TryGetValue("screen_name", out screenName))
					acct.Name = screenName;

				// Call original delegate if it was set
				originalProcessExtras?.Invoke(extras, acct);
			});

			return base.AuthenticateAsync(options);
		}
	}
}

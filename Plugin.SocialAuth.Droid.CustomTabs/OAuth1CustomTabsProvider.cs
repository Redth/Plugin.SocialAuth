using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.CustomTabs;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.Droid.CustomTabs
{
	public class OAuth1CustomTabsProvider : IAuthProviderDroidWithCallback<IOAuth1Account, IOAuth1Options>
	{
		static CustomTabsAuthSession authSession;
		static OAuth1 oauth;
		static TaskCompletionSource<IOAuth1Account> tcsAuth;

		public bool Callback(Intent intent, Bundle state)
		{
			if (authSession == null || intent == null)
				return false;

			var uri = new Uri (intent.Data.ToString());

			// Only handle schemes we expect
			if (!WebUtil.CanHandleCallback(oauth.CallbackUrl, uri))
				return false;

			var resp = oauth.ParseCallback(uri);

			IDictionary<string, string> items = null;

			var accessTokenTask = Task.Run(async () =>
			{
				items = await oauth.GetAccessTokenAsync(resp);
			});

			accessTokenTask.Wait();

			var account = oauth.GetAccountFromResponse<OAuth1Account>(items);

			if (account == null)
				tcsAuth?.TrySetException(new Exception("Failed to parse server response."));
			else
				tcsAuth?.TrySetResult(account);

			oauth = null;
			authSession = null;

			return true;
		}

		public virtual async Task<IOAuth1Account> AuthenticateAsync(IOAuth1Options options)
		{
			// Set the current expected redirect uri
			oauth = new OAuth1(options); 

			var url = await oauth.GetInitialUrlAsync();

			tcsAuth = new TaskCompletionSource<IOAuth1Account>();

			// DO TABS
			authSession = new CustomTabsAuthSession();
			authSession.AuthTaskCompletionSource = new TaskCompletionSource<IOAuth1Account>();

			authSession.ParentActivity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;
			authSession.CustomTabsActivityManager = new CustomTabsActivityManager(authSession.ParentActivity);
			authSession.CustomTabsActivityManager.NavigationEvent += (navigationEvent, extras) =>
			{
				if (navigationEvent == CustomTabsCallback.TabHidden)
				{
					if (authSession.AuthTaskCompletionSource != null)
						authSession.AuthTaskCompletionSource.TrySetCanceled();
				}

			};
			authSession.CustomTabsActivityManager.CustomTabsServiceConnected += delegate
			{
				var builder = new CustomTabsIntent.Builder(authSession.CustomTabsActivityManager.Session)
												  .SetShowTitle(true);

				var customTabsIntent = builder.Build();
				customTabsIntent.Intent.AddFlags(Android.Content.ActivityFlags.SingleTop | ActivityFlags.NoHistory | ActivityFlags.NewTask);

				CustomTabsHelper.AddKeepAliveExtra(authSession.ParentActivity, customTabsIntent.Intent);

				customTabsIntent.LaunchUrl(authSession.ParentActivity, Android.Net.Uri.Parse(url.AbsoluteUri));
			};

			if (!authSession.CustomTabsActivityManager.BindService())
				tcsAuth.TrySetException(new Exception("CustomTabs not supported."));
			
			return await tcsAuth.Task;
		}

		public Task LogoutAsync()
		{
			return Task.CompletedTask;
		}

		class CustomTabsAuthSession
		{
			public CustomTabsActivityManager CustomTabsActivityManager { get; set; }
			public Android.Support.V4.App.FragmentActivity ParentActivity { get; set; }
			public TaskCompletionSource<IOAuth1Account> AuthTaskCompletionSource { get; set; }
		}
	}
}

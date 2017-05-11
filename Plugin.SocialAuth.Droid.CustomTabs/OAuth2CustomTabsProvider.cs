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
	public class OAuth2CustomTabsProvider : IAuthProviderDroidWithCallback<IOAuth2Account, IOAuth2Options>
	{
		static CustomTabsAuthSession authSession;
		static OAuth2 oauth;
		static TaskCompletionSource<IOAuth2Account> tcsAuth;

		public bool Callback(Intent intent, Bundle state)
		{
			if (authSession == null || intent == null)
				return false;

			var uri = new Uri(intent.Data.ToString());

			// Only handle schemes we expect
			if (!WebUtil.CanHandleCallback(oauth.RedirectUrl, uri))
				return false;

			IOAuth2Account account = null;
			var accessTokenTask = Task.Run(async () =>
			{
				account = await oauth.ParseCallbackAsync<OAuth2Account>(uri);
			});
			accessTokenTask.Wait();

			if (account == null)
				tcsAuth?.TrySetException(new Exception("Failed to parse server response."));
			else
				tcsAuth?.TrySetResult(account);

			oauth = null;
			authSession = null;

			return true;
		}

		public async Task<IOAuth2Account> AuthenticateAsync(IOAuth2Options options)
		{
			// Set the current expected redirect uri
			oauth = new OAuth2(options);

			var url = await oauth.GetInitialUrlAsync();


			tcsAuth = new TaskCompletionSource<IOAuth2Account>();

			// DO TABS
			authSession = new CustomTabsAuthSession();
			authSession.AuthTaskCompletionSource = new TaskCompletionSource<IOAuth2Account>();

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
			public TaskCompletionSource<IOAuth2Account> AuthTaskCompletionSource { get; set; }
		}
	}
}

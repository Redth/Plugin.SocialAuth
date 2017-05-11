using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Plugin.SocialAuth;
using SafariServices;
using UIKit;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.iOS
{
	public class OAuth1SafariServicesProvider : IAuthProvideriOS<IOAuth1Account, IOAuth1Options>
	{
		static OAuth1 oauth = null;

		public bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (oauth == null || oauth.CallbackUrl == null)
				return false;

			// Make sure we're redirecting from our SFSafariViewController and not some other app
			if (sourceApplication != "com.apple.SafariViewService")
				return false;

			var uri = new Uri(url.AbsoluteString);

			// Only handle schemes we expect
			if (!WebUtil.CanHandleCallback(oauth.CallbackUrl, uri))
				return false;

			var resp = oauth.ParseCallback(uri);

			var waitReq = new System.Threading.ManualResetEventSlim();

			IDictionary<string, string> items = null;

			var accessTokenTask = Task.Run(async () =>
			{
				items = await oauth.GetAccessTokenAsync(resp);
			});

			accessTokenTask.Wait();

			var account = oauth.GetAccountFromResponse<OAuth1Account>(items);

			Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController.InvokeOnMainThread(async () =>
				await Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController.DismissViewControllerAsync(true));

			if (account == null)
				tcsAuth?.TrySetException(new Exception("Failed to parse server response."));
			else
				tcsAuth?.TrySetResult(account);

			// Reset oauth for next request
			oauth = null;

			return true;
		}


		static TaskCompletionSource<IOAuth1Account> tcsAuth;

		public virtual async Task<IOAuth1Account> AuthenticateAsync(IOAuth1Options options)
		{
			// Set the current expected redirect uri
			oauth = new OAuth1(options);

			var url = await oauth.GetInitialUrlAsync();

			var svc = new SFSafariViewController(NSUrl.FromString(url.AbsoluteUri))
			{
				Delegate = new NativeSFSafariViewControllerDelegate
				{
					DidFinishHandler = (obj) =>
					{
						tcsAuth.TrySetResult(default(IOAuth1Account));
					}
				}
			};

			tcsAuth = new TaskCompletionSource<IOAuth1Account>();


			await Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController.PresentViewControllerAsync(svc, true);


			return await tcsAuth.Task;
		}

		public Task LogoutAsync()
		{
			return Task.CompletedTask;
		}

		class NativeSFSafariViewControllerDelegate : SFSafariViewControllerDelegate
		{
			public Action<SFSafariViewController> DidFinishHandler { get; set; }

			public override void DidFinish(SFSafariViewController controller)
			{
				DidFinishHandler?.Invoke(controller);
			}
		}
	}
}

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
	public class OAuth2SafariServicesProvider : IAuthProvideriOS<IOAuth2Account, IOAuth2Options>
	{
		static OAuth2 oauth;

		public bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (oauth == null || oauth.RedirectUrl == null)
				return false;

			// Make sure we're redirecting from our SFSafariViewController and not some other app
			if (sourceApplication != "com.apple.SafariViewService")
				return false;

			var uri = new Uri(url.AbsoluteString);

			// Only handle schemes we expect
			if (!WebUtil.CanHandleCallback(oauth.RedirectUrl, uri))
				return false;

			IOAuth2Account account = null;
			var accessTokenTask = Task.Run(async () =>
			{
				account = await oauth.ParseCallbackAsync<OAuth2Account>(uri);
			});
			accessTokenTask.Wait();

			Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController.InvokeOnMainThread(async () =>
				await Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController.DismissViewControllerAsync(true));

			if (account == null)
				tcsAuth?.TrySetException(new Exception("Failed to parse server response."));
			else
				tcsAuth?.TrySetResult(account);

			// Reset this for the next request
			oauth = null;

			return true;
		}

		static TaskCompletionSource<IOAuth2Account> tcsAuth;

		public async Task<IOAuth2Account> AuthenticateAsync(IOAuth2Options options)
		{
			oauth = new OAuth2(options);

			var url = await oauth.GetInitialUrlAsync();

			var svc = new SFSafariViewController(NSUrl.FromString(url.AbsoluteUri))
			{
				Delegate = new NativeSFSafariViewControllerDelegate
				{
					DidFinishHandler = (obj) =>
					{
						tcsAuth.TrySetResult(default(IOAuth2Account));
					}
				}
			};

			tcsAuth = new TaskCompletionSource<IOAuth2Account>();


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

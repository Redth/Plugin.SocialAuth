using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Google.Core;
using Google.SignIn;
using Plugin.SocialAuth.iOS;
using UIKit;

namespace Plugin.SocialAuth.Google.Native.iOS
{
	public class GoogleAuthProvider : NSObject, IAuthProvideriOS<IGoogleAccount, IGoogleAuthOptions>
	{
		static MyDelegate myDelegate;
		static MyUIDelegate myUIDelegate;

		static TaskCompletionSource<GoogleUser> tcsSignin;

		public bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return SignIn.SharedInstance.HandleUrl(url, sourceApplication, annotation);
		}

		public static bool FinishedLaunching(UIApplication app, NSDictionary options, IGoogleAuthOptions authOptions)
		{
			NSError configureError;

			Firebase.Analytics.App.Configure();

			Context.SharedInstance.Configure(out configureError);

			SetSignInOptions(authOptions);

			myDelegate = new MyDelegate();
			myUIDelegate = new MyUIDelegate();

			SignIn.SharedInstance.Delegate = myDelegate;
			SignIn.SharedInstance.UIDelegate = myUIDelegate;

			return true;
		}

		static void SetSignInOptions(IGoogleAuthOptions options)
		{
			SignIn.SharedInstance.ClientID = options.ClientId;
			SignIn.SharedInstance.ServerClientID = options.ServerClientId;
			SignIn.SharedInstance.ShouldFetchBasicProfile = options.FetchProfile;
			if (options.Scopes != null && options.Scopes.Length > 0)
				SignIn.SharedInstance.Scopes = options.Scopes;
		}

		public async Task<IGoogleAccount> AuthenticateAsync(IGoogleAuthOptions options)
		{
			tcsSignin = new TaskCompletionSource<GoogleUser>();

			SetSignInOptions(options);

			GoogleUser user = null;

			// First try silently
			try
			{
				SignIn.SharedInstance.SignInUserSilently();

				user = await tcsSignin.Task;
			}
			catch
			{

				// If silent failed, try the normal sign in
				tcsSignin = new TaskCompletionSource<GoogleUser>();

				SignIn.SharedInstance.SignInUser();

				user = await tcsSignin.Task;
			}

			if (user == null || user.Authentication == null)
				return null;

			var googleAuth = new global::Google.SignIn.Authentication();
			Authentication tokens = null;

			try
			{
				tokens = await googleAuth.GetTokensAsync();
			}
			catch { }

			DateTime? accessTokenExpires = null;
			if (tokens?.AccessTokenExpirationDate != null)
				accessTokenExpires = (DateTime)tokens.AccessTokenExpirationDate;

			DateTime? idTokenExpires = null;
			if (tokens?.IdTokenExpirationDate != null)
				idTokenExpires = (DateTime)tokens.IdTokenExpirationDate;

			DateTime? refreshTokenExpires = null;
			if (!string.IsNullOrEmpty(tokens?.RefreshToken))
				refreshTokenExpires = DateTime.MaxValue;

			return new GoogleAccount
			{
				Id = user.UserID,
				IdToken = user.Authentication?.IdToken,
				IdTokenExpires = idTokenExpires,
				AccessToken = tokens?.AccessToken,
				AccessTokenExpires = accessTokenExpires,
				RefreshToken = tokens?.RefreshToken,
				RefreshTokenExpires = refreshTokenExpires,
				ServerAuthCode = user.ServerAuthCode,
				DisplayName = user.Profile?.Name,
				Email = user.Profile?.Email,
				FamilyName = user.Profile?.FamilyName,
				GivenName = user.Profile?.GivenName,
				GrantedScopes = user.AccessibleScopes ?? new string[] { },
				PhotoUri = user?.Profile?.GetImageUrl(options.RequestedPhotoDimension),
			};
		}

		public Task LogoutAsync()
		{
			SignIn.SharedInstance.SignOutUser();

			return Task.CompletedTask;
		}

		class MyDelegate : global::Google.SignIn.SignInDelegate
		{
			public override void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
			{
				if (error != null)
					tcsSignin.TrySetException(new Exception(error.LocalizedDescription));
				else
					tcsSignin?.TrySetResult(user);
			}
		}

		class MyUIDelegate : SignInUIDelegate
		{
			public override void WillDispatch(SignIn signIn, NSError error)
			{
				Console.WriteLine(error?.LocalizedDescription);
			}

			public override void PresentViewController(SignIn signIn, UIViewController viewController)
			{
				var vc = global::Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController;
				vc.PresentModalViewController(viewController, true);
			}

			public override void DismissViewController(SignIn signIn, UIViewController viewController)
			{
				var vc = global::Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController;
				vc.DismissModalViewController(true);
			}
		}
	}

	static class GoogleAuthenticationExtensions
	{
		public static Task<Authentication> GetTokensAsync(this Authentication auth)
		{
			var tcsAuth = new TaskCompletionSource<Authentication>();

			auth.GetTokens((auth2, error) =>
			{
				if (error != null)
					tcsAuth.TrySetException(new Exception(error.LocalizedDescription));
				else
					tcsAuth.TrySetResult(auth2);
			});

			return tcsAuth.Task;
		}
	}
}

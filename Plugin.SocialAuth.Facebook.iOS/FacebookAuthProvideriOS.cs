using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Plugin.SocialAuth.Facebook.iOS
{
	public class FacebookAuthProvider : IAuthProvider<IFacebookAccount, IFacebookAuthOptions> 
	{
		public static void Init (string facebookAppId)
		{
			global::Facebook.CoreKit.Settings.AppID = facebookAppId;

			Registrar.Register (ProviderType.Facebook, typeof(FacebookAuthProvider));
		}

		public static void Uninit ()
		{
			Registrar.Unregister (ProviderType.Facebook, typeof(FacebookAuthProvider));
		}

		public static bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return global::Facebook.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}

		public static bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			return global::Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);
		}

		public async Task<IFacebookAccount> AuthenticateAsync (IFacebookAuthOptions options)
		{
			var currentAccessToken = global::Facebook.CoreKit.AccessToken.CurrentAccessToken;

			if (currentAccessToken != null 
			    && (currentAccessToken.ExpirationDate == null || ((DateTime)currentAccessToken.ExpirationDate).ToUniversalTime () > DateTime.UtcNow)) {
				var currentProfile = global::Facebook.CoreKit.Profile.CurrentProfile;

				if (currentProfile != null)
					populateAccount (currentAccessToken, currentProfile, options.RequestedPhotoSize);
			}

			var loginManager = new global::Facebook.LoginKit.LoginManager ();

			var vc = global::Plugin.SocialAuth.iOS.SocialAuth.PresentingViewController;

			global::Facebook.LoginKit.LoginManagerLoginResult resp = null;

			var scopes = options?.Scopes ?? new string [] {};

			if (options?.WritePermissions ?? false)
				resp = await loginManager.LogInWithPublishPermissionsAsync (scopes, vc);
			else
				resp = await loginManager.LogInWithReadPermissionsAsync (scopes, vc);
			
			if (resp.IsCancelled)
				return null;

			if (resp == null || resp.Token == null)
				return null;

			return populateAccount(resp.Token, global::Facebook.CoreKit.Profile.CurrentProfile, options.RequestedPhotoSize);
		}

		public Task LogoutAsync ()
		{
			var loginManager = new global::Facebook.LoginKit.LoginManager ();
			loginManager.LogOut ();

			return Task.CompletedTask;
		}

		IFacebookAccount populateAccount (global::Facebook.CoreKit.AccessToken accessToken, global::Facebook.CoreKit.Profile profile, ImageSize photoSize = null)
		{
			if (accessToken == null)
				return null;
			
			DateTime? expires = null;
			if (accessToken.ExpirationDate != null)
				expires = (DateTime)accessToken.ExpirationDate;
			DateTime? refreshed = null;
			if (accessToken.RefreshDate != null)
				refreshed = (DateTime)accessToken.RefreshDate;

			var permissions = new List<string> ();
			if (accessToken.Permissions != null)
				foreach (var p in accessToken.Permissions.ToArray<NSString> ())
					permissions.Add (p.ToString ());

			var declinedPermissions = new List<string> ();
			if (accessToken.DeclinedPermissions != null)
				foreach (var p in accessToken.DeclinedPermissions.ToArray<NSString> ())
					declinedPermissions.Add (p.ToString ());

			Uri photoUri = null;
			if (profile != null) {
				if (photoSize == null)
					photoSize = new ImageSize();
				var u = profile.ImageUrl (
					photoSize.IsSquare ? global::Facebook.CoreKit.ProfilePictureMode.Square : global::Facebook.CoreKit.ProfilePictureMode.Normal,
					new CoreGraphics.CGSize (photoSize.Width, photoSize.Height));
				if (u != null)
					photoUri = new Uri (u.AbsoluteString, UriKind.Absolute);
			}

			Uri linkUri = null;
			if (profile?.LinkUrl != null)
				linkUri = new Uri (profile.LinkUrl.AbsoluteString, UriKind.Absolute);

			return new FacebookAccount {
				Id = profile?.UserID ?? accessToken.UserID,
				ApplicationId = accessToken.AppID,
				UserId = profile?.UserID ?? accessToken.UserID,
				AccessToken = accessToken.TokenString,
				AccessTokenExpires = expires,
				Permissions = permissions,
				DeclinedPermissions = declinedPermissions,
				Name = profile?.Name,
				FirstName = profile?.FirstName,
				LastName = profile?.LastName,
				MiddleName = profile?.MiddleName,
				PhotoUri = photoUri,
				LinkUri = linkUri
			};
		}
	}
}

using System;
using System.Threading.Tasks;
using Android.App;
using Java.Interop;
using Plugin.SocialAuth.Droid;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace Plugin.SocialAuth.Facebook.Native.Droid
{
	public class FacebookAuthProvider : IAuthProviderDroidWithOnActivityResult<IFacebookAccount, IFacebookAuthOptions>
	{
		public static void Init (string facebookAppId)
		{
			FacebookSdk.ApplicationId = facebookAppId;
		}

		public void OnActivityResult(int requestCode, Result resultCode, global::Android.Content.Intent data)
		{
			callbackManager?.OnActivityResult(requestCode, (int)resultCode, data);
		}

		static ICallbackManager callbackManager = null;

		public async Task<IFacebookAccount> AuthenticateAsync(IFacebookAuthOptions options)
		{
			// Check if we have a cached login already that's still good
			var currentAccessToken = AccessToken.CurrentAccessToken;
			if (currentAccessToken != null && (currentAccessToken.Expires == null || ToManagedDateTime(currentAccessToken.Expires) > DateTime.UtcNow))
			{
				// If we also have a profile, we have enough information to return a good account
				// without actuall doing the sign in flow
				var currentProfile = Profile.CurrentProfile;
				if (currentProfile != null)
					return populateAccount(currentAccessToken, currentProfile, options.RequestedPhotoSize);
			}

			var activity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;

			callbackManager = CallbackManagerFactory.Create();
			var loginManager = LoginManager.Instance;
			var fbHandler = new FbCallbackHandler();

			loginManager.RegisterCallback(callbackManager, fbHandler);


			fbHandler.Reset();

			if (options?.WritePermissions ?? false)
				loginManager.LogInWithPublishPermissions(activity, options?.Scopes);
			else
				loginManager.LogInWithReadPermissions(activity, options?.Scopes);

			LoginResult result = null;

			try
			{
				result = await fbHandler.Task;
			}
			catch (Exception ex)
			{
				throw ex;
			}

			if (result == null || result.AccessToken == null)
				return null;

			return populateAccount(result.AccessToken, Profile.CurrentProfile, options.RequestedPhotoSize);
		}

		public Task LogoutAsync()
		{
			LoginManager.Instance.LogOut();

			return Task.CompletedTask;
		}

		DateTime ToManagedDateTime(Java.Util.Date date)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(date.Time);
		}


		IFacebookAccount populateAccount(AccessToken accessToken, Profile profile, ImageSize photoSize = null)
		{
			DateTime? expires = null;
			if (accessToken.Expires != null)
				expires = ToManagedDateTime(accessToken.Expires);
			//DateTime? lastRefresh = null;
			//if (accessToken.LastRefresh != null)
			//	lastRefresh = ToManagedDateTime (accessToken.LastRefresh);

			Uri linkUri = null;
			if (profile?.LinkUri != null)
				linkUri = new Uri(profile.LinkUri.ToString(), UriKind.Absolute);

			Uri photoUri = null;
			if (profile != null)
			{
				if (photoSize == null)
					photoSize = new ImageSize();
				var u = profile.GetProfilePictureUri(photoSize.Width, photoSize.Height);
				if (u != null)
					photoUri = new Uri(u.ToString(), UriKind.Absolute);
			}

			return new FacebookAccount
			{
				Id = profile?.Id ?? accessToken.UserId,
				ApplicationId = accessToken.ApplicationId,
				UserId = accessToken.UserId,
				Permissions = accessToken.Permissions,
				DeclinedPermissions = accessToken.DeclinedPermissions,
				AccessToken = accessToken.Token,
				AccessTokenExpires = expires,
				Name = profile?.Name,
				FirstName = profile?.FirstName,
				MiddleName = profile?.MiddleName,
				LastName = profile?.LastName,
				LinkUri = linkUri,
				PhotoUri = photoUri,
			};
		}

		class FbCallbackHandler : Java.Lang.Object, IFacebookCallback
		{
			TaskCompletionSource<LoginResult> tcs = new TaskCompletionSource<LoginResult>();

			public void Reset()
			{
				if (tcs != null && !tcs.Task.IsCompleted)
				{
					tcs.TrySetResult(null);
				}

				tcs = new TaskCompletionSource<LoginResult>();
			}

			public Task<LoginResult> Task
			{
				get
				{
					return tcs.Task;
				}
			}
			public void OnCancel()
			{
				tcs.TrySetResult(null);
			}

			public void OnError(FacebookException ex)
			{
				tcs.TrySetException(new System.Exception(ex.Message));
			}

			public void OnSuccess(Java.Lang.Object data)
			{
				tcs.TrySetResult(data.JavaCast<LoginResult>());
			}
		}
	}
}

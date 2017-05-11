using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Extensions;
using Android.OS;
using Plugin.SocialAuth.Droid;

namespace Plugin.SocialAuth.Google.Droid
{
	public class GoogleAuthProvider : IAuthProviderDroidWithOnActivityResult<IAccount, IGoogleAuthOptions>
	{
		public void OnActivityResult(int requestCode, Result result, Intent data)
		{
			// Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
			if (requestCode == GoogleSignInProvider.SIGN_IN_REQUEST_CODE)
			{
				var googleSignInResult = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);

				googleSignInProvider?.FoundResult(googleSignInResult);
			}
		}

		static GoogleSignInProvider googleSignInProvider;

		public async Task<IAccount> AuthenticateAsync(IGoogleAuthOptions options)
		{
			googleSignInProvider = new GoogleSignInProvider();

			GoogleSignInResult result = null;

			result = await googleSignInProvider.Authenticate(options);

			if (result == null || result.Status.IsCanceled || result.Status.IsInterrupted)
				return null;

			if (!result.IsSuccess || result.SignInAccount == null)
			{
				// TODO: Report error
				return null;
			}

			var acct = result.SignInAccount;

			var grantedScopes = new List<string>();
			if (acct.GrantedScopes != null && acct.GrantedScopes.Any())
				grantedScopes.AddRange(acct.GrantedScopes.Select(s => s.ToString()));

			Uri photoUrl = null;
			if (acct.PhotoUrl != null)
				Uri.TryCreate(acct.PhotoUrl.ToString(), UriKind.Absolute, out photoUrl);


			// Try and obtain an access token
			var activity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;
			var androidAccount = Android.Accounts.AccountManager.FromContext(activity)
										?.GetAccounts()
										?.FirstOrDefault(a => a.Name?.Equals(result?.SignInAccount?.Email, StringComparison.InvariantCultureIgnoreCase) ?? false);

			var tokenScopes = options.Scopes.Select(s => "oauth2:" + s);

			string accessToken = null;
			DateTime? accessTokenExpires = null;
			if (androidAccount != null)
			{
				// We must run this on a non-ui thread or google will throw an error
				accessToken = await Task.Run(() =>
				{
					try
					{
						return global::Android.Gms.Auth.GoogleAuthUtil.GetToken(activity, androidAccount, string.Join(" ", tokenScopes));
					}
					catch { }
					return null;
				});

				// This token will be an offline token 
				if (!string.IsNullOrEmpty(accessToken))
					accessTokenExpires = DateTime.MaxValue;
			}

			return new GoogleAccount
			{
				Id = acct.Id,
				IdToken = acct.IdToken,
				ServerAuthCode = acct.ServerAuthCode,
				AccessToken = accessToken,
				AccessTokenExpires = accessTokenExpires,
				DisplayName = acct.DisplayName,
				Email = acct.Email,
				FamilyName = acct.FamilyName,
				GivenName = acct.GivenName,
				GrantedScopes = grantedScopes,
				PhotoUri = photoUrl
			};
		}

		public async Task LogoutAsync()
		{
			var googleSignOutProvider = new GoogleSignOutProvider();

			await googleSignOutProvider.SignOut();
		}

		internal class GoogleSignInProvider : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener, GoogleApiClient.IConnectionCallbacks
		{
			internal const int SIGN_IN_REQUEST_CODE = 41221;

			GoogleApiClient googleApiClient;

			TaskCompletionSource<GoogleSignInResult> tcsSignIn;

			public void FoundResult(GoogleSignInResult result)
			{
				if (tcsSignIn != null && !tcsSignIn.Task.IsCompleted)
					tcsSignIn.TrySetResult(result);
			}

			public async Task<GoogleSignInResult> Authenticate(IGoogleAuthOptions options)
			{
				var googleScopes = options?.Scopes?.Select(s => new Scope(s))?.ToArray();

				var gsoBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn);

				if (!string.IsNullOrEmpty(options.ServerClientId))
					gsoBuilder = gsoBuilder.RequestIdToken(options.ServerClientId);

				if (googleScopes != null && googleScopes.Any())
					gsoBuilder = gsoBuilder.RequestScopes(googleScopes.First(), googleScopes.Skip(1).ToArray());

				if (options.FetchProfile)
					gsoBuilder = gsoBuilder.RequestProfile().RequestEmail();

				var gso = gsoBuilder.Build();

				var activity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;

				googleApiClient = new GoogleApiClient.Builder(activity)
													 .EnableAutoManage(activity, 789, this)
													 .AddConnectionCallbacks(this)
													 .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
													 .Build();

				googleApiClient.Connect();

				tcsSignIn = new TaskCompletionSource<GoogleSignInResult>();

				var result = await tcsSignIn.Task;

				googleApiClient.StopAutoManage(activity);

				return result;
			}

			public void OnConnectionFailed(ConnectionResult result)
			{
				tcsSignIn.TrySetException(new Exception("Google Services Connection Failed: " + result.ErrorMessage));
			}

			public async void OnConnected(Bundle connectionHint)
			{
				// First try a silent login
				try
				{
					var silentResult = await Auth.GoogleSignInApi.SilentSignIn(googleApiClient).AsAsync<GoogleSignInResult>();

					if (silentResult != null && silentResult.IsSuccess)
					{
						FoundResult(silentResult);
						return;
					}

				}
				catch { }

				// If silent failed, go to the normal sign in flow
				var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);

				var activity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;

				activity.StartActivityForResult(signInIntent, SIGN_IN_REQUEST_CODE);
			}

			public void OnConnectionSuspended(int cause)
			{
				tcsSignIn.TrySetException(new Exception("Connection Suspended: " + cause));
			}
		}

		internal class GoogleSignOutProvider : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener, GoogleApiClient.IConnectionCallbacks
		{
			internal const int SIGN_IN_REQUEST_CODE = 41221;

			GoogleApiClient googleApiClient;

			TaskCompletionSource<Statuses> tcsSignOut;

			public async Task<Statuses> SignOut()
			{
				var gsoBuilder = new Android.Gms.Auth.Api.SignIn.GoogleSignInOptions.Builder(Android.Gms.Auth.Api.SignIn.GoogleSignInOptions.DefaultSignIn);
				var gso = gsoBuilder.Build();

				var activity = Plugin.SocialAuth.Droid.SocialAuth.CurrentActivity;

				googleApiClient = new GoogleApiClient.Builder(activity)
													 .EnableAutoManage(activity, 987, this)
													 .AddConnectionCallbacks(this)
													 .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
													 .Build();

				googleApiClient.Connect();

				tcsSignOut = new TaskCompletionSource<Statuses>();

				var result = await tcsSignOut.Task;

				googleApiClient.StopAutoManage(activity);

				return result;
			}

			public void OnConnectionFailed(ConnectionResult result)
			{
				tcsSignOut.TrySetException(new Exception("Google Services Connection Failed: " + result.ErrorMessage));
			}

			public async void OnConnected(Bundle connectionHint)
			{
				var r = await Auth.GoogleSignInApi.SignOut(googleApiClient).AsAsync<Statuses>();
				tcsSignOut.TrySetResult(r);
			}

			public void OnConnectionSuspended(int cause)
			{
				tcsSignOut.TrySetException(new Exception("Connection Suspended: " + cause));
			}
		}
	}
}

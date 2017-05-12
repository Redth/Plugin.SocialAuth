using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.SocialAuth;
using Plugin.SocialAuth.Google;
using Plugin.SocialAuth.Twitter;
using Plugin.SocialAuth.OAuth;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.SocialAuth.Facebook;
using Plugin.SocialAuth;
using Plugin.SocialAuth.Facebook.Native;
using Plugin.SocialAuth.Google.Native;

namespace SocialAuthSample
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();
		}

		async Task HandleResults(IAccount account)
		{
			if (account == null)
				await DisplayAlert("Auth Failed", "Authentication Failed!", "OK");
			else
				await DisplayAlert("Authenticated!", "Name: " + (account.Name ?? account.Id), "OK");
		}

		async void Facebook_Clicked(object sender, EventArgs e)
		{
			var existingAccount = SocialAuthManager.Current.AccountStore.FindAnyAccount(ProviderType.Facebook);

			var account = await SocialAuthManager.Current.AuthenticateFacebookAsync(new FacebookAuthOptions(),
																				  existingAccount?.Id);

			await HandleResults(account);
		}

		async void Google_Clicked(object sender, EventArgs e)
		{
			var existingAccount = SocialAuthManager.Current.AccountStore.FindAnyAccount(ProviderType.Google);

			var account = await SocialAuthManager.Current.AuthenticateGoogleAsync(new GoogleAuthOptions
			{
				ClientId = SocialKeys.GOOGLE_CLIENT_ID,
				ServerClientId = SocialKeys.GOOGLE_SERVER_CLIENT_ID,
			}, existingAccount?.Id);

			await HandleResults(account);
		}

		async void Twitter_Clicked(object sender, EventArgs e)
		{
			// You may choose to look for *any* existing Twitter account if you didn't store the 
			// previously used account ID somewhere
			var existingAccount = SocialAuthManager.Current.AccountStore.FindAnyAccount(ProviderType.Twitter);

			var account = await SocialAuthManager.Current.AuthenticateTwitterAsync(new TwitterAuthOptions
			{
				ConsumerKey = SocialKeys.TWITTER_CONSUMER_KEY,
				ConsumerSecret = SocialKeys.TWITTER_CONSUMER_SECRET,
				CallbackUrl = new Uri("plugin.socialauth://twitter"),
			}, existingAccount?.Id);

			await HandleResults(account);
		}

		async void Fitbit_Clicked(object sender, EventArgs e)
		{
			// You may choose to look for *any* existing Twitter account if you didn't store the 
			// previously used account ID somewhere
			var existingAccount = SocialAuthManager.Current.AccountStore.FindAnyAccount("fitbit");

			var account = await SocialAuthManager.Current.AuthenticateOAuth2Async("fitbit", new OAuth2Options
			{
				ClientId = SocialKeys.FITBIT_CLIENT_ID,
				ClientSecret = SocialKeys.FITBIT_CLIENT_SECRET,
				AuthorizeUrl = new Uri("https://www.fitbit.com/oauth2/authorize"),
				AccessTokenUrl = new Uri("https://api.fitbit.com/oauth2/token"),
				RedirectUrl = new Uri("plugin.socialauth://fitbit"),
				Scope = "activity",
				SendAccessTokenRequestAuthHeader = true, // Fitbit requires the Authorization header
				ProcessExtras = (extras, acct) =>
				{
					// The response may have extra key/value pairs we are interested in
					acct.Id = extras?["user_id"];
				}
			}, existingAccount?.Id);

			await HandleResults(account);
		}
	}
}

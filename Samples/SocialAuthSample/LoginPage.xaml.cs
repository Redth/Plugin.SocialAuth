using System;
using System.Collections.Generic;
using Plugin.SocialAuth;
using Xamarin.Forms;
using Plugin.SocialAuth.Facebook;
using Plugin.SocialAuth.Google;

namespace SocialAuthSample
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			authManager = new SocialAuthManager();
		}

		SocialAuthManager authManager;

		async void Facebook_Clicked (object sender, EventArgs e)
		{
			try {
				var account = await authManager.FacebookAuthenticateAsync (new FacebookAuthOptions ());

				await DisplayAlert ("Facebook", $"Authenticated as: {account.Name} ({account.Id})", "OK");
			} catch (Exception ex) {
				await DisplayAlert ("Failed", $"Authentication Failed: {ex.Message}", "OK");
			}
		}

		async void Google_Clicked (object sender, EventArgs e)
		{
			try {
				var account = await authManager.GoogleAuthenticateAsync (new GoogleAuthOptions {
					ClientId = "your-android-app-client-id",
					ServerClientId = "your-oauth-web-app-client-id",
				});

				await DisplayAlert ("Google", $"Authenticated as: {account.Name} ({account.Id})", "OK");
			} catch (Exception ex) {
				await DisplayAlert ("Failed", $"Authentication Failed: {ex.Message}", "OK");
			}
		}
	}
}

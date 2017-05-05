﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SocialAuthSample
{
	public partial class App : Xamarin.Forms.Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage (new LoginPage ()) { BarBackgroundColor = Color.LightSlateGray };
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

	}
}

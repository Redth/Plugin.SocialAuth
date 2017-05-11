using System.Collections.Generic;

using Xamarin.Forms;

namespace SocialAuthSample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Current.MainPage = new NavigationPage(new LoginPage())
			{
				BarBackgroundColor = (Color)Current.Resources["Primary"],
				BarTextColor = Color.White
			};
		}
	}
}

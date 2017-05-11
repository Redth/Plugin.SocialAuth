using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace Plugin.SocialAuth.Droid
{
	public interface IAuthProviderDroidWithOnActivityResult<TAccount, TOptions> : IAuthProvider<TAccount, TOptions>
		where TAccount : IAccount
		where TOptions : IAuthOptions
	{
		void OnActivityResult(int requestCode, Result resultCode, Intent data);
	}
}

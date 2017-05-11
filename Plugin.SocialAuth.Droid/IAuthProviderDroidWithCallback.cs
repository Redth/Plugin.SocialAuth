using System;
using Android.Content;
using Android.OS;

namespace Plugin.SocialAuth.Droid
{
	public interface IAuthProviderDroidWithCallback<TAccount, TOptions> : IAuthProvider<TAccount, TOptions>
		where TAccount : IAccount
		where TOptions : IAuthOptions
	{
		bool Callback(Intent intent, Bundle state);
	}
}

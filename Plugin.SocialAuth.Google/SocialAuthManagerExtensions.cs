using System.Threading.Tasks;
using Plugin.SocialAuth.Google;

namespace Plugin.SocialAuth
{
	public static class GoogleSocialAuthManagerExtensions
	{
		public static Task<IGoogleAccount> AuthenticateGoogleAsync(this SocialAuthManager manager, IGoogleAuthOptions options, string accountId = null)
		{
			return manager.AuthenticateAsync<IGoogleAccount, IGoogleAuthOptions>(ProviderType.Google, options, accountId);
		}

		public static Task LogoutGoogleAsync(this SocialAuthManager manager, IGoogleAccount account)
		{
			return manager.LogoutAsync<IGoogleAccount, IGoogleAuthOptions>(ProviderType.Google, account);
		}
	}
}

using System.Threading.Tasks;

namespace Plugin.SocialAuth.Google
{
	public static class SocialAuthManagerExtensions
	{
		public static Task<IGoogleAccount> GoogleAuthenticateAsync (this SocialAuthManager manager, IGoogleAuthOptions options)
		{
			return manager.AuthenticateAsync<IGoogleAccount, IGoogleAuthOptions> (ProviderType.Google, options);
		}

		public static Task GoogleLogoutAsync (this SocialAuthManager manager, IGoogleAccount account)
		{
			return manager.LogoutAsync<IGoogleAccount, IGoogleAuthOptions> (ProviderType.Google, account);
		}
	}
}

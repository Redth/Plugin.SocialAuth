using System.Threading.Tasks;

namespace Plugin.SocialAuth.Google
{
	public static class SocialAuthManagerExtensions
	{
		public static Task<IGoogleAccount> GoogleAuthenticateAsync (this SocialAuthManager manager, IGoogleAuthOptions options)
		{
			return manager.AuthenticateAsync<IGoogleAccount, IGoogleAuthOptions> (ProviderType.Google, options);
		}
	}
}

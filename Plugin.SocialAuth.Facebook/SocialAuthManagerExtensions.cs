using System.Threading.Tasks;

namespace Plugin.SocialAuth.Facebook
{
	public static class SocialAuthManagerExtensions
	{
		public static Task<IFacebookAccount> FacebookAuthenticateAsync (this SocialAuthManager manager, IFacebookAuthOptions options)
		{
			return manager.AuthenticateAsync<IFacebookAccount, IFacebookAuthOptions> (ProviderType.Facebook, options);
		}
	}
}

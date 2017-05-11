using System.Threading.Tasks;

namespace Plugin.SocialAuth.Facebook
{
	public static class SocialAuthManagerExtensions
	{
		public static Task<IFacebookAccount> AuthenticateFacebookAsync(this SocialAuthManager manager, IFacebookAuthOptions options, string accountId = null)
		{
			return manager.AuthenticateAsync<IFacebookAccount, IFacebookAuthOptions>(ProviderType.Facebook, options, accountId);
		}

		public static Task LogoutFacebookAsync(this SocialAuthManager manager, IFacebookAccount account)
		{
			return manager.LogoutAsync<IFacebookAccount, IFacebookAuthOptions>(ProviderType.Facebook, account);
		}
	}
}

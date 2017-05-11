using System.Threading.Tasks;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth
{
	public static class OAuthSocialAuthManagerExtensions
	{
		public static Task<IOAuth1Account> AuthenticateOAuth1Async(this SocialAuthManager manager, string providerTypeId, IOAuth1Options options, string accountId = null)
		{
			return manager.AuthenticateAsync<IOAuth1Account, IOAuth1Options>(providerTypeId, options, accountId);
		}

		public static Task<IOAuth1Account> AuthenticateOAuth1Async(this SocialAuthManager manager, ProviderType providerType, IOAuth1Options options, string accountId = null)
		{
			return manager.AuthenticateAsync<IOAuth1Account, IOAuth1Options>(providerType, options, accountId);
		}

		public static Task<IOAuth2Account> AuthenticateOAuth2Async(this SocialAuthManager manager, string providerTypeId, IOAuth2Options options, string accountId = null)
		{
			return manager.AuthenticateAsync<IOAuth2Account, IOAuth2Options>(providerTypeId, options, accountId);
		}

		public static Task<IOAuth2Account> AuthenticateOAuth2Async(this SocialAuthManager manager, ProviderType providerType, IOAuth2Options options, string accountId = null)
		{
			return manager.AuthenticateAsync<IOAuth2Account, IOAuth2Options>(providerType, options, accountId);
		}
	}
}

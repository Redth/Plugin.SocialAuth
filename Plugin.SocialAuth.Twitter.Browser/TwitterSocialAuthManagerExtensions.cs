using System;
using System.Threading.Tasks;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth
{
	public static class TwitterSocialAuthManagerExtensions
	{
		public static Task<IOAuth1Account> AuthenticateTwitterAsync(this SocialAuthManager authManager, IOAuth1Options options, string accountId = null)
		{
			return authManager.AuthenticateAsync<IOAuth1Account, IOAuth1Options>(SocialAuthManager.GetProviderTypeId(ProviderType.Twitter), options, accountId);
		}
	}
}
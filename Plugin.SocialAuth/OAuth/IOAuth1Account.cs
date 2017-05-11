using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Plugin.SocialAuth.OAuth
{
	public interface IOAuth1Account : IAccount
	{
		string AccessToken { get; set; }
		string TokenSecret { get; set; }
	}

	public class OAuth1Account : Account, IOAuth1Account
	{
		public string AccessToken
		{
			get { return GetStringValue("access_token"); }
			set { SetStringValue("access_token", value); }
		}

		public string TokenSecret
		{
			get { return GetStringValue("token_secret"); }
			set { SetStringValue("token_secret", value); }
		}

		public override Task<bool> CheckValidity()
		{
			var valid = true;

			// Missing access token AND id token, so no tokens? login again.
			if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(TokenSecret))
				valid = false;

			return Task.FromResult(valid);
		}
	}
}

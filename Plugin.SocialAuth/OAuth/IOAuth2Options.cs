using System;
using System.Collections.Generic;

namespace Plugin.SocialAuth.OAuth
{
	public delegate void OAuth2ProcessExtrasDelegate(IDictionary<string, string> extras, IOAuth2Account account);

	public interface IOAuth2Options : IAuthOptions
	{
		string ClientId { get; set; }
		string ClientSecret { get; set; }
		string Scope { get; set; }
		Uri AuthorizeUrl { get; set; }
		Uri RedirectUrl { get; set; }
		Uri AccessTokenUrl { get; set; }

		bool SendAccessTokenRequestAuthHeader { get; set; }

		OAuth2ProcessExtrasDelegate ProcessExtras { get; set; }
	}

}

using System;
using System.Collections.Generic;

namespace Plugin.SocialAuth.OAuth
{
	public class OAuth2Options : AuthOptions, IAuthOptions, IOAuth2Options
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Scope { get; set; }
		public Uri AuthorizeUrl { get; set; }
		public Uri RedirectUrl { get; set; }
		public Uri AccessTokenUrl { get; set; }

		/// <summary>
		/// Some OAuth providers require that a `Authorization: Basic ` header be sent during the access token request
		/// with a value of Base64 (clientId + ":" + clientSecret)
		/// </summary>
		/// <value><c>true</c> if send access token request auth header; otherwise, <c>false</c>.</value>
		public bool SendAccessTokenRequestAuthHeader { get; set; }

		public OAuth2ProcessExtrasDelegate ProcessExtras { get; set; }
	}

}

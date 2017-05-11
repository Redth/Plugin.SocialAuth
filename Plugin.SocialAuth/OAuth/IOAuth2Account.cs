using System;

namespace Plugin.SocialAuth.OAuth
{
	public interface IOAuth2Account : IAccount
	{
		string AccessToken { get; set; }
		string RefreshToken { get; set; }
		string TokenType { get; set; }
		string Scope { get; set; }
		//string IdToken { get; set; }

		DateTime? AccessTokenExpires { get; set; }
		DateTime? RefreshTokenExpires { get; set; }
		//DateTime? IdTokenExpires { get; set; }
	}
}

using System;
using System.Collections.Generic;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.Google.Native
{
	public interface IGoogleAccount : IOAuth2Account
	{
		string DisplayName { get; }
		string Email { get; }
		string GivenName { get; }
		string FamilyName { get; }
		string ServerAuthCode { get; }
		Uri PhotoUri { get; }

		IEnumerable<string> GrantedScopes { get; }

		string IdToken { get; set; }
		DateTime? IdTokenExpires { get; set; }
	}
}

using System;
using System.Collections.Generic;

namespace Plugin.SocialAuth.Google
{
	public interface IGoogleAccount : IAccount
	{
		string DisplayName { get; }
		string Email { get; }
		string GivenName { get; }
		string FamilyName { get; }
		string ServerAuthCode { get; }
		Uri PhotoUri { get; }

		IEnumerable<string> GrantedScopes { get; }
	}
}

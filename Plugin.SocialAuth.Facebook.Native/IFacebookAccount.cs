using System;
using System.Collections.Generic;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.Facebook.Native
{
	public interface IFacebookAccount : IOAuth2Account
	{
		string ApplicationId { get; set; }
		string UserId { get; set; }

		string FirstName { get; set; }
		string MiddleName { get; set; }
		string LastName { get; set; }

		IEnumerable<string> DeclinedPermissions { get; set; }
		IEnumerable<string> Permissions { get; set; }

		Uri LinkUri { get; set; }
		Uri PhotoUri { get; set; }
	}
}

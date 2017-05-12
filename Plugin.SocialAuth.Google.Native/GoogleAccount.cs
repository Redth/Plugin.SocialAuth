using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.Google.Native
{
	public class GoogleAccount : OAuth2Account, IGoogleAccount
	{
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public string GivenName { get; set; }
		public string FamilyName { get; set; }
		public string ServerAuthCode { get; set; }
		public Uri PhotoUri { get; set; }
		public IEnumerable<string> GrantedScopes { get; set; }

		public string IdToken { get; set; }
		public DateTime? IdTokenExpires { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.SocialAuth.Google
{
	public class GoogleAccount : Account, IGoogleAccount
	{
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public string GivenName { get; set; }
		public string FamilyName { get; set; }
		public string ServerAuthCode { get; set; }
		public Uri PhotoUri { get; set; }
		public IEnumerable<string> GrantedScopes { get; set; }
	}
}

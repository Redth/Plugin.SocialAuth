using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth.Facebook.Native
{
	public class FacebookAccount : OAuth2Account, IFacebookAccount
	{
		public string ApplicationId { get; set; }
		public string UserId { get; set; }

		public IEnumerable<string> DeclinedPermissions { get; set; }
		public IEnumerable<string> Permissions { get; set; }

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }

		public Uri LinkUri { get; set; }
		public Uri PhotoUri { get; set; }
	}
}

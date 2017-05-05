using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.SocialAuth.Facebook
{
	public class FacebookAccount : Account, IAccount, IFacebookAccount
	{
		public string ApplicationId { get;set; }
		public string UserId { get; set; }

		public IEnumerable<string> DeclinedPermissions { get;set; }
		public IEnumerable<string> Permissions { get;set; }

		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }

		public Uri LinkUri { get; set; }
		public Uri PhotoUri { get; set; }
	}
}

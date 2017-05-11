using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.SocialAuth.OAuth
{

	public class OAuth1Options : AuthOptions, IAuthOptions, IOAuth1Options
	{
		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string Scope { get; set; }
		public Uri AuthorizeUrl { get; set; }
		public Uri CallbackUrl { get; set; }
		public Uri AccessTokenUrl { get; set; }
		public Uri RequestTokenUrl { get; set; }

		public OAuth1ProcessExtrasDelegate ProcessExtras { get; set; }
	}

}

using System;
using System.Collections.Generic;

namespace Plugin.SocialAuth.OAuth
{
	public delegate void OAuth1ProcessExtrasDelegate(IDictionary<string, string> extras, IOAuth1Account account);

	public interface IOAuth1Options : IAuthOptions
	{
		string ConsumerKey { get; set; }
		string ConsumerSecret { get; set; }
		string Scope { get; set; }
		Uri AuthorizeUrl { get; set; }
		Uri CallbackUrl { get; set; }
		Uri AccessTokenUrl { get; set; }
		Uri RequestTokenUrl { get; set; }

		OAuth1ProcessExtrasDelegate ProcessExtras { get; set; }
	}

}
